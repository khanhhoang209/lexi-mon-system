using LexiMon.Repository.Domains;
using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Mappers;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LexiMon.Service.Implements;

public class OrderService : IOrderService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<ServiceResponse> CreateOrder(OrderRequestDto request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"User {userId} not found");
                return new ServiceResponse { Succeeded = false, Message = "User not found!" };
            }

            var validationResponse = await ValidateAndResolveTargetAsync(request, cancellationToken);
            if (!validationResponse.Succeeded)
            {
                return validationResponse;
            }

            // Lấy giá
            var pricing = await ResolvePricingAsync(request, cancellationToken);
            if (pricing is null)
                return new ServiceResponse { Succeeded = false, Message = "Pricing not found!" };

            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var order = request.ToOrder(userId, pricing.PurchaseCost, pricing.CoinCost);

            await orderRepo.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new ResponseData<Guid>()
            {
                Succeeded = true,
                Message = "Order created successfully.",
                Data = order.Id
            };


        }
        catch (Exception e)
        {
            _logger.LogError(e, "CreateOrder failed");
            return new ServiceResponse { Succeeded = false, Message = "Create order failed!" };
        }
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

    #region Helpers

    /// <summary>
    /// Kiểm tra rằng request chỉ chọn đúng 1 mục tiêu (Course hoặc Item)
    /// </summary>
    private async Task<ServiceResponse> ValidateAndResolveTargetAsync(
        OrderRequestDto request,
        CancellationToken cancellationToken)
    {
        // Chỉ 1 trong 2: CourseId XOR ItemId
        var hasCourse = request.CourseId.HasValue;
        var hasItem = request.ItemId.HasValue;
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var query = orderRepo.Query();
        if (!hasCourse && !hasItem)
        {
            _logger.LogError("CreateOrder: neither CourseId nor ItemId provided");
            return new ServiceResponse
            {
                Succeeded = false,
                Message = "Either CourseId or ItemId must be provided!"
            };
        }

        if (hasCourse && hasItem)
        {
            _logger.LogError("CreateOrder: both CourseId and ItemId provided");
            return new ServiceResponse
            {
                Succeeded = false,
                Message = "CreateOrder: both CourseId and ItemId provided!"
            };
        }

        if (hasItem)
        {
            var itemRepo = _unitOfWork.GetRepository<Item, Guid>();
            var item = await itemRepo.GetByIdAsync(request.ItemId!.Value, cancellationToken);
            if (item == null)
            {
                _logger.LogError("CreateOrder: Item {ItemId} not found", request.ItemId);
                return new ServiceResponse
                {
                    Succeeded = false,
                    Message = "Item not found!"
                };
            }

            query = query.Where(o => o.ItemId == request.ItemId);
            if (query.Any())
            {
                _logger.LogError("CreateOrder: Item {ItemId} already purchased", request.ItemId);
                return new ServiceResponse
                {
                    Succeeded = false,
                    Message = "Item already purchased!"
                };
            }
            else
            {
                var courseRepo = _unitOfWork.GetRepository<Course, Guid>();
                var course = await courseRepo.GetByIdAsync(request.CourseId!.Value, cancellationToken);
                if (course == null)
                {
                    _logger.LogError("CreateOrder: Course {CourseId} not found", request.CourseId);
                    return new ServiceResponse
                    {
                        Succeeded = false,
                        Message = "Course not found!"
                    };
                }

                query = query.Where(o => o.CourseId == request.CourseId);
                if (query.Any())
                {
                    _logger.LogError("CreateOrder: Course {CourseId} already purchased", request.CourseId);
                    return new ServiceResponse
                    {
                        Succeeded = false,
                        Message = "Course already purchased!"
                    };
                }
            }
        }
        return new ServiceResponse { Succeeded = true, Message = "Validation passed." };
        
    }
    
    private sealed record PricingInfo(decimal? PurchaseCost, decimal? CoinCost);
    private async Task<PricingInfo?> ResolvePricingAsync(
        OrderRequestDto request, 
        CancellationToken ct)
    {
        if (request.ItemId.HasValue)
        {
            var itemRepo = _unitOfWork.GetRepository<Item, Guid>();
            var item = await itemRepo.GetByIdAsync(request.ItemId.Value, ct);
            if (item is null) return null;
            
            return new PricingInfo(
                PurchaseCost: item.Price,
                CoinCost:     item.Coin   
            );
        }

        if (request.CourseId.HasValue)
        {
            var courseRepo = _unitOfWork.GetRepository<Course, Guid>();
            var course = await courseRepo.GetByIdAsync(request.CourseId.Value, ct);
            if (course is null) return null;

            return new PricingInfo(
                PurchaseCost: course.Price,
                CoinCost:     course.Coin   
            );
        }

        return null;
    }

    #endregion
}