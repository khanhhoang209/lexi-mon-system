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

        var expiredAt = DateTimeOffset.UtcNow.AddSeconds(_payOsSetings.ExpirationSeconds).ToUnixTimeSeconds();

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

    public async Task<ServiceResponse> HandleWebhook(string transaction, CancellationToken cancellationToken = default)
    {
        var validData = IsValidData(transaction);
        if (!validData)
        {
            _logger.LogWarning("Invalid webhook data: {Transaction}", transaction);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Dữ liệu không hợp lệ!"
            };
        }

        var envelope = JObject.Parse(transaction);

        var data = envelope["data"] as JObject;
        if (data == null)
        {
            _logger.LogError("Data field is missing in webhook payload");
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Dữ liệu không hợp lệ!"
            };
        }
        var orderCode = data["orderCode"]?.ToObject<long>();

        var transactionRepo = _unitOfWork.GetRepository<Transaction, Guid>();
        var transactionEntity = await transactionRepo.Query()
            .FirstOrDefaultAsync(t => t.OrderCode == orderCode, cancellationToken);
        if (transactionEntity == null)
        {
            _logger.LogError("Transaction not found for OrderCode: {OrderCode}", orderCode);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Giao dịch không tồn tại!"
            };
        }

        transactionEntity.TransactionStatus = TransactionStatus.Return;
        _logger.LogInformation("Transaction updated to Return for OrderCode: {OrderCode}", orderCode);
        await transactionRepo.UpdateAsync(transactionEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

    private static string ComputeHmacSha256Hex(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}