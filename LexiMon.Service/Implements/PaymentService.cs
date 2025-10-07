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
            description: "Thanh toán đơn hàng",
            items: items,
            returnUrl: $"{_payOsSetings.BaseUrl}/payments/return?orderCode={orderCode}",
            cancelUrl: $"{_payOsSetings.BaseUrl}/payments/cancel?orderCode={orderCode}",
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

    public async Task<ServiceResponse> PaymentReturn(long orderCode, CancellationToken cancellationToken = default)
    {
        var transactionRepo = _unitOfWork.GetRepository<Transaction, Guid>();
        var transaction = await transactionRepo.Query()
            .FirstOrDefaultAsync(t => t.OrderCode == orderCode, cancellationToken);

        if (transaction == null) {
            _logger.LogError("Order not found with OrderCode: {OrderCode}", orderCode);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Không tìm thấy giao dịch!"
            };
        }

        if (transaction.TransactionStatus != TransactionStatus.Pending)
        {
            _logger.LogError("Payment return {@TransactionStatus}", transaction.TransactionStatus);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Giao dịch không hợp lệ!"
            };
        }

        transaction.TransactionStatus = TransactionStatus.Return;
        await transactionRepo.UpdateAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Payment return success: {@transaction}", transaction);
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = "Thanh toán thành công!"
        };
    }

    public async Task<ServiceResponse> PaymentCancel(long orderCode, CancellationToken cancellationToken = default)
    {
        var transactionRepo = _unitOfWork.GetRepository<Transaction, Guid>();
        var transaction = await transactionRepo.Query()
            .FirstOrDefaultAsync(t => t.OrderCode == orderCode, cancellationToken);

        if (transaction == null) {
            _logger.LogError("Order not found with OrderCode: {OrderId}", orderCode);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Không tìm thấy giao dịch!"
            };
        }

        if (transaction.TransactionStatus != TransactionStatus.Pending)
        {
            _logger.LogError("Payment return {@TransactionStatus}", transaction.TransactionStatus);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Giao dịch không hợp lệ!"
            };
        }

        transaction.TransactionStatus = TransactionStatus.Return;
        await transactionRepo.UpdateAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Payment return success: {@transaction}", transaction);
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = "Hủy thanh toán thành công!"
        };
    }
}