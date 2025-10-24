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
using Transaction = LexiMon.Repository.Domains.Transaction;

namespace LexiMon.Service.Implements;

public class PaymentService : IPaymentService
{
    private readonly PayOS _payOs;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PayOsSetings _payOsSetings;
    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IUnitOfWork unitOfWork, ILogger<PaymentService> logger, IOptions<PayOsSetings> payOsSetings, IOrderService orderService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _orderService = orderService;
        _payOsSetings = payOsSetings.Value;
        _payOs = new PayOS(_payOsSetings!.ClientId, _payOsSetings.ApiKey, _payOsSetings.ChecksumKey);
    }

    public async Task<ServiceResponse> CreatePayment(PaymentRequest requestBody, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check existing order
            var order = await _unitOfWork.GetRepository<Order, Guid>()
                .Query()
                .Include(o => o.Item)
                .Include(o => o.Course)
                .FirstOrDefaultAsync(o => o.Id == requestBody.OrderId, cancellationToken);

            if (order == null)
            {
                _logger.LogError("Order not found with OrderId: {OrderId}", requestBody.OrderId);
                return new ServiceResponse()
                {
                    Succeeded = false,
                    Message = "Không tìm thấy đơn hàng!"
                };
            }

            if(order.CreatedAt < DateTimeOffset.UtcNow.AddMinutes(-20))
            {
                _logger.LogError("Order expired with OrderId: {OrderId}", requestBody.OrderId);
                order.PaymentStatus = PaymentStatus.Fail;
                await _unitOfWork.GetRepository<Order, Guid>().UpdateAsync(order, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return new ServiceResponse()
                {
                    Succeeded = false,
                    Message = "Đơn hàng đã hết hạn! Vui lòng mua đơn hàng mới."
                };
            }
            
            if (order.PaymentStatus != PaymentStatus.Pending)
            {
                _logger.LogError("Order status is not valid with OrderId: {OrderId}", requestBody.OrderId);
                return new ServiceResponse()
                {
                    Succeeded = false,
                    Message = "Trạng thái đơn hàng không hợp lệ!"
                };
            }

            // Information for payment
            var itemName = "";
            if (order.Item != null) itemName = order.Item.Name;
            if (order.Course != null) itemName = order.Course.Title;
            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var item = new ItemData(itemName, 1, (int)order.PurchaseCost!);
            var items = new List<ItemData> { item };
            var expiredAt = TimeConverter.GetCurrentVietNamTime().AddSeconds(_payOsSetings.ExpirationSeconds).ToUnixTimeSeconds();

            var data = new PaymentData(
                orderCode: orderCode,
                amount: item.price,
                description: "Thanh toán đơn hàng",
                items: items,
                returnUrl: $"{_payOsSetings.BaseUrl}/payments/return?orderId={order.Id}",
                cancelUrl: $"{_payOsSetings.BaseUrl}/payments/cancel?orderId={order.Id}",
                expiredAt: expiredAt
            );
            var response = await _payOs.createPaymentLink(data);

            // Save transaction
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
            await _unitOfWork.GetRepository<Transaction, Guid>().AddAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Transaction created successfully with OrderId: {OrderId}", transaction.OrderId);
            return new ResponseData<CreatePaymentResult>()
            {
                Succeeded = true,
                Message = "Tạo liên kết thanh toán thành công!",
                Data = response
            };
        }
        catch
        {
            _logger.LogError("Error creating payment link for OrderId: {OrderId}", requestBody.OrderId);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Lỗi khi tạo liên kết thanh toán!"
            };
        }

    }

    public async Task<ServiceResponse> PaymentReturn(Guid orderId, CancellationToken cancellationToken = default)
    {
        var transaction = await _unitOfWork.GetRepository<Transaction, Guid>()
            .Query()
            .FirstOrDefaultAsync(t => t.OrderId == orderId, cancellationToken);

        if (transaction == null)
        {
            _logger.LogError("Order not found with OrderId: {OrderId}", orderId);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Không tìm thấy đơn hàng!"
            };
        }

        if (transaction.TransactionStatus != TransactionStatus.Pending)
        {
            _logger.LogError("Transaction status not valid with {OrderId}", orderId);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Trạng thái giao dịch không hợp lệ!"
            };
        }

        try
        {
            var orderResponse = await _orderService.UpdateOrderToReturn(transaction.OrderId, cancellationToken);
            if (!orderResponse.Succeeded)
            {
                transaction.TransactionStatus = TransactionStatus.Fail;
                await _unitOfWork.GetRepository<Transaction, Guid>().UpdateAsync(transaction, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Payment return fail with OrderId: {OrderId}", transaction.OrderId);
                return new ServiceResponse()
                {
                    Succeeded = false,
                    Message = "Giao dịch thất bại!"
                };
            }

            transaction.TransactionStatus = TransactionStatus.Return;
            await _unitOfWork.GetRepository<Transaction, Guid>().UpdateAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Payment return sucessfully with OrderId: {OrderId}", transaction.OrderId);
            return new ServiceResponse()
            {
                Succeeded = true,
                Message = "Giao dịch thành công!"
            };
        }
        catch
        {
            transaction.TransactionStatus = TransactionStatus.Fail;
            await _unitOfWork.GetRepository<Transaction, Guid>().UpdateAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Payment return fail with OrderId: {OrderId}", transaction.OrderId);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Giao dịch thất bại!"
            };
        }

    }

    public async Task<ServiceResponse> PaymentCancel(Guid orderId, CancellationToken cancellationToken = default)
    {
        var transaction = await _unitOfWork.GetRepository<Transaction, Guid>()
            .Query()
            .FirstOrDefaultAsync(t => t.OrderId == orderId, cancellationToken);

        if (transaction == null) {
            _logger.LogError("Order not found with OrderId: {OrderId}", orderId);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Không tìm thấy đơn hàng!"
            };
        }

        if (transaction.TransactionStatus != TransactionStatus.Pending)
        {
            _logger.LogError("Transaction status not valid with {OrderId}", orderId);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Trạng thái giao dịch không hợp lệ!"
            };
        }

        try
        {
            var orderResponse = await _orderService.UpdateOrderToCancel(transaction.OrderId, cancellationToken);
            if (!orderResponse.Succeeded)
            {
                transaction.TransactionStatus = TransactionStatus.Fail;
                await _unitOfWork.GetRepository<Transaction, Guid>().UpdateAsync(transaction, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Payment cancel fail with OrderId: {OrderId}", transaction.OrderId);
                return new ServiceResponse()
                {
                    Succeeded = false,
                    Message = "Giao dịch thất bại!"
                };
            }

            transaction.TransactionStatus = TransactionStatus.Cancel;
            await _unitOfWork.GetRepository<Transaction, Guid>().UpdateAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Payment cancel sucessfully with OrderId: {OrderId}", transaction.OrderId);
            return new ServiceResponse()
            {
                Succeeded = true,
                Message = "Hủy giao dịch thành công!"
            };
        }
        catch
        {
            transaction.TransactionStatus = TransactionStatus.Fail;
            await _unitOfWork.GetRepository<Transaction, Guid>().UpdateAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Payment cancel fail with OrderId: {OrderId}", transaction.OrderId);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Giao dịch thất bại!"
            };
        }
    }
}