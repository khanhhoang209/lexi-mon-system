using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;

namespace LexiMon.Service.Interfaces;

public interface IOrderService
{
    Task<ServiceResponse> CreateOrder(OrderRequestDto request, 
        string userId, 
        CancellationToken cancellationToken = default);
    Task<ServiceResponse> GetOrders();
    Task<ServiceResponse> GetOrderById(int orderId);
    Task<ServiceResponse> UpdateOrderToReturn(int orderId);
    Task<ServiceResponse> UpdateOrderToCancel(int orderId);
}