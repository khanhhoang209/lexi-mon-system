using System.Security.Cryptography;
using System.Text;
using LexiMon.Repository.Domains;
using LexiMon.Repository.Enum;
using LexiMon.Repository.Interfaces;
using LexiMon.Repository.Utils;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Configs;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json.Linq;
using Transaction = LexiMon.Repository.Domains.Transaction;

namespace LexiMon.Service.Implements;

public class PaymentService : IPaymentService
{
    private readonly PayOS _payOs;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PayOsSetings _payOsSetings;
    private readonly ILogger<PaymentService> _logger;


    public PaymentService(IUnitOfWork unitOfWork, ILogger<PaymentService> logger, IOptions<PayOsSetings> payOsSetings)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _payOsSetings = payOsSetings.Value;
        _payOs = new PayOS(_payOsSetings!.ClientId, _payOsSetings.ApiKey, _payOsSetings.ChecksumKey);
    }

    public async Task<ServiceResponse> CreatePayment(PaymentRequest requestBody, CancellationToken cancellationToken = default)
    {
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await orderRepo.Query()
                .Include(o => o.Item)
                .Include(o => o.Course)
                .FirstOrDefaultAsync(o => o.Id == requestBody.OrderId, cancellationToken);
        if (order == null)
        {
            _logger.LogError("Order not found: {OrderId}", requestBody.OrderId);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Không tìm thấy đơn hàng!"
            };
        }

        var itemName = "";
        if (order.Item != null)
        {
            itemName = order.Item.Name;
        }
        if (order.Course != null)
        {
            itemName = order.Course.Title;
        }

        var orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var item = new ItemData(itemName, 1, (int)order.PurchaseCost!);
        var items = new List<ItemData> { item };
        var expiredAt = TimeConverter.GetCurrentVietNamTime().AddSeconds(_payOsSetings.ExpirationSeconds).ToUnixTimeSeconds();

        var data = new PaymentData(
            orderCode: orderCode,
            amount: item.price,
            description: $"{itemName}",
            items: items,
            returnUrl: $"{_payOsSetings.BaseUrl}/return",
            cancelUrl: $"{_payOsSetings.BaseUrl}/cancel",
            expiredAt: expiredAt
        );

        var response = await _payOs.createPaymentLink(data);

        var transactionRepo = _unitOfWork.GetRepository<Transaction, Guid>();
        var transaction = new Transaction()
        {
            OrderId = order.Id,
            UserId = order.UserId,
            OrderCode = orderCode,
            PaymentLinkId = response.paymentLinkId,
            CheckoutUrl = response.checkoutUrl,
            QrCode = response.qrCode,
            Amount = (decimal)order.PurchaseCost,
            Description = data.description,
            TransactionStatus = TransactionStatus.Pending,
            PaymentMethod = PaymentMethod.PayOs,
        };
        await transactionRepo.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("CreatePayment: {@data}", data);
        return new ResponseData<CreatePaymentResult>()
        {
            Succeeded = true,
            Message = "Tạo liên kết thanh toán thành công!",
            Data = response
        };
    }

    public async Task<ServiceResponse> HandleWebhook(WebhookType webhookType, CancellationToken cancellationToken = default)
    {
        var webhookData = _payOs.verifyPaymentWebhookData(webhookType);

        if (webhookData == null!)
        {
            _logger.LogError("Invalid webhook data");
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Dữ liệu webhook không hợp lệ!"
            };
        }

        _logger.LogInformation("Webhook: {WebhookType}", webhookType);

        var transactionRepo = _unitOfWork.GetRepository<Transaction, Guid>();
        var transaction = await transactionRepo.Query()
            .FirstOrDefaultAsync(t => t.OrderCode == webhookData.orderCode, cancellationToken);

        if (transaction == null)
        {
            _logger.LogError("Order not found: {OrderCode}", webhookData.orderCode);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Không tìm thấy giao dịch!"
            };
        }

        if (webhookData.code == "00")
        {
            transaction.TransactionStatus = TransactionStatus.Return;
        }

        if (webhookData.code is "20" or "401")
        {
            transaction.TransactionStatus = TransactionStatus.Fail;
        }

        await transactionRepo.UpdateAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated transaction: {@transaction}", transaction);
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = "Xử lý webhook thành công!"
        };
    }


    public bool IsValidData(string transaction)
    {
        try
        {
            // Parse JSON
            var envelope = JObject.Parse(transaction);

            var data = envelope["data"] as JObject;
            var signature = envelope["signature"]?.ToString();
            var checksumKey = _payOsSetings.ChecksumKey;

            // Sort keys alphabetically
            var sortedKeys = data!.Properties()
                .Select(p => p.Name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToList();

            // Build key=value&key=value string
            var sb = new StringBuilder();
            for (var i = 0; i < sortedKeys.Count; i++)
            {
                var key = sortedKeys[i];
                var value = data[key]?.ToString() ?? string.Empty;
                sb.Append($"{key}={value}");
                if (i < sortedKeys.Count - 1)
                    sb.Append("&");
            }

            // Compute HMAC-SHA256
            var computedSignature = ComputeHmacSha256Hex(sb.ToString(), checksumKey);

            return string.Equals(computedSignature, signature, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error verifying signature: {ex.Message}");
            return false;
        }
    }

    private string ComputeHmacSha256Hex(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    public string GenerateSignature(IDictionary<string, object> data, string checksumKey)
    {
        var sortedKeys = data.Keys.OrderBy(k => k, StringComparer.Ordinal).ToList();

        var sb = new StringBuilder();
        for (int i = 0; i < sortedKeys.Count; i++)
        {
            var key = sortedKeys[i];
            var value = data[key]?.ToString() ?? string.Empty;
            sb.Append($"{key}={value}");
            if (i < sortedKeys.Count - 1)
                sb.Append("&");
        }

        var dataQueryStr = sb.ToString();

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataQueryStr));

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}