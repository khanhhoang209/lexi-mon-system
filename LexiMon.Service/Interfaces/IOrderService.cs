using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

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
    Task<PaginatedResponse<OrderUserResponseDto>> GetAllUsersOrdersByUserId(
        GetOrderUserRequest request,
        string userId, 
        CancellationToken cancellationToken);
    Task<PaginatedResponse<OrderResponseDto>> GetAllOrders(
        GetOrderRequest request,
        CancellationToken cancellationToken);
}