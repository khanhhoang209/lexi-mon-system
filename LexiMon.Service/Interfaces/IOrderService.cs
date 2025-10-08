using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;

namespace LexiMon.Service.Interfaces;

public interface IOrderService
{
    Task<ServiceResponse> CreateOrder(OrderRequestDto request, 
        string userId, 
        CancellationToken cancellationToken = default);
    Task<ServiceResponse> GetOrders();
    Task<ServiceResponse> GetOrderById(Guid orderId, CancellationToken cancellationToken = default);
    Task<ServiceResponse> UpdateOrderToReturn(Guid orderId, CancellationToken cancellationToken = default);
    Task<ServiceResponse> UpdateOrderToCancel(Guid orderId, CancellationToken cancellationToken = default);
}