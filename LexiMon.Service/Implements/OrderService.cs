using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
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

    public Task<ServiceResponse> GetOrderById(int orderId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse> UpdateOrderToReturn(int orderId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse> UpdateOrderToCancel(int orderId)
    {
        throw new NotImplementedException();
    }
}