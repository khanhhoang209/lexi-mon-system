using LexiMon.Repository.Domains;
using LexiMon.Repository.Enum;
using LexiMon.Repository.Interfaces;
using LexiMon.Repository.Utils;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LexiMon.Service.Implements;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task<ServiceResponse> CreateOrder()
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse> GetOrders()
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse> GetOrderById(Guid orderId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateOrderToReturn(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            _logger.LogError("Order not found with OrderId: {OrderId}", orderId);
            return new ServiceResponse
            {
                Succeeded = false,
                Message = "Không tìm thấy đơn hàng!"
            };
        }

        if (order.PaymentStatus != PaymentStatus.Pending)
        {
            _logger.LogError("Order status not valid with OrderId: {OrderId}", orderId);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Trạng thái đơn hàng không hợp lệ!"
            };
        }

        try
        {
            order.PaymentStatus = PaymentStatus.Return;
            order.PaidAt = TimeConverter.GetCurrentVietNamTime();

            if (order.Course != null)
            {
                var userDeck = new UserDeck()
                {
                    UserId = order.UserId,
                    CourseId = order.CourseId,
                };
                await _unitOfWork.GetRepository<UserDeck, Guid>().AddAsync(userDeck, cancellationToken);
            }

            if (order.Item != null)
            {
                var character = await _unitOfWork.GetRepository<Character, Guid>()
                    .Query()
                    .FirstOrDefaultAsync(c => c.UserId == order.UserId, cancellationToken);

                var equipment = new Equipment()
                {
                    CharacterId = character!.Id,
                    ItemId = (Guid)order.ItemId!,
                };
                await _unitOfWork.GetRepository<Equipment, (Guid, Guid)>().AddAsync(equipment, cancellationToken);
            }

            await _unitOfWork.GetRepository<Order, Guid>().UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Order return successfully with OrderId: {OrderId}", orderId);
            return new ServiceResponse()
            {
                Succeeded = true,
                Message = "Thanh toán đơn hàng thành công!"
            };
        }
        catch (Exception ex)
        {
            order.PaymentStatus = PaymentStatus.Fail;
            await _unitOfWork.GetRepository<Order, Guid>().UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Order return fail with OrderId: {OrderId}", order.Id);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Thanh toán đơn hàng thất bại!"
            };
        }
    }

    public async Task<ServiceResponse> UpdateOrderToCancel(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            _logger.LogError("Order not found with OrderId: {OrderId}", orderId);
            return new ServiceResponse
            {
                Succeeded = false,
                Message = "Không tìm thấy đơn hàng!"
            };
        }

        if (order.PaymentStatus != PaymentStatus.Pending)
        {
            _logger.LogError("Order status not valid with OrderId: {OrderId}", orderId);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Trạng thái đơn hàng không hợp lệ!"
            };
        }

        try
        {
            order.PaymentStatus = PaymentStatus.Cancel;
            await _unitOfWork.GetRepository<Order, Guid>().UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Order cancel successfully with OrderId: {OrderId}", orderId);
            return new ServiceResponse()
            {
                Succeeded = true,
                Message = "Hủy đơn hàng thành công!"
            };
        }
        catch (Exception ex)
        {
            order.PaymentStatus = PaymentStatus.Fail;
            await _unitOfWork.GetRepository<Order, Guid>().UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Order cancel fail with OrderId: {OrderId}", order.Id);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Hủy đơn hàng thất bại!"
            };
        }
    }
}