using LexiMon.Service.ApiResponse;

namespace LexiMon.Service.Interfaces;

public interface IOrderService
{
    Task<ServiceResponse> CreateOrder();
    Task<ServiceResponse> GetOrders();
    Task<ServiceResponse> GetOrderById(int orderId);
    Task<ServiceResponse> UpdateOrderToReturn(int orderId);
    Task<ServiceResponse> UpdateOrderToCancel(int orderId);
}