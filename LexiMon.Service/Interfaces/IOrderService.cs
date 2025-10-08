using LexiMon.Service.ApiResponse;

namespace LexiMon.Service.Interfaces;

public interface IOrderService
{
    Task<ServiceResponse> CreateOrder();
    Task<ServiceResponse> GetOrders();
    Task<ServiceResponse> GetOrderById(Guid orderId, CancellationToken cancellationToken = default);
    Task<ServiceResponse> UpdateOrderToReturn(Guid orderId, CancellationToken cancellationToken = default);
    Task<ServiceResponse> UpdateOrderToCancel(Guid orderId, CancellationToken cancellationToken = default);
}