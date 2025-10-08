using LexiMon.Repository.Domains;
using LexiMon.Repository.Enum;
using LexiMon.Repository.Interfaces;
using LexiMon.Repository.Utils;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
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

            var validationResponse = await ValidateAndResolveTargetAsync(request, userId, cancellationToken);
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
        catch
        {
            _logger.LogError("CreateOrder failed");
            return new ServiceResponse { Succeeded = false, Message = "Create order failed!" };
        }
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
        var order = await _unitOfWork.GetRepository<Order, Guid>()
            .Query()
            .Include(o => o.Course)
            .Include(o => o.Item)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
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
        catch
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
        catch
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

    #region Helpers

    /// <summary>
    /// Kiểm tra rằng request chỉ chọn đúng 1 mục tiêu (Course hoặc Item)
    /// </summary>
    private async Task<ServiceResponse> ValidateAndResolveTargetAsync(
        OrderRequestDto request,
        string userId,                 
        CancellationToken ct)
    {
        var hasCourse = request.CourseId.HasValue;
        var hasItem   = request.ItemId.HasValue;

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
                Message = "Only one of CourseId or ItemId can be provided!"
            };
        }

        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();

        if (hasItem)
        {
            var itemRepo = _unitOfWork.GetRepository<Item, Guid>();
            var item = await itemRepo.GetByIdAsync(request.ItemId!.Value, ct);
            if (item is null)
            {
                _logger.LogError("CreateOrder: Item {ItemId} not found", request.ItemId);
                return new ServiceResponse
                {
                    Succeeded = false, 
                    Message = "Item not found!"
                };
            }

            // 2) Trùng theo user + item (chặn khi Pending or Return)
            var dupItem = await orderRepo.Query()
                .AnyAsync(o => o.UserId == userId
                               && o.ItemId == request.ItemId
                               && (o.PaymentStatus == PaymentStatus.Pending || o.PaymentStatus == PaymentStatus.Return)
                , ct);
            if (dupItem)
            {
                _logger.LogError("CreateOrder: Item {ItemId} already purchased (pending or paid) by user {UserId}", request.ItemId, userId);
                return new ServiceResponse { Succeeded = false, Message = "Item already purchased (pending or paid)!" };
            }
        }
        else // hasCourse
        {
            var courseRepo = _unitOfWork.GetRepository<Course, Guid>();
            var course = await courseRepo.GetByIdAsync(request.CourseId!.Value, ct);
            if (course is null)
            {
                _logger.LogError("CreateOrder: Course {CourseId} not found", request.CourseId);
                return new ServiceResponse { Succeeded = false, Message = "Course not found!" };
            }

            //Trùng user + course (chặn Pending or Return)
            var dupCourse = await orderRepo.Query()
                .AnyAsync(o => o.UserId == userId
                            && o.CourseId == request.CourseId
                            && (o.PaymentStatus == PaymentStatus.Pending || o.PaymentStatus == PaymentStatus.Return)
                    , ct);
            if (dupCourse)
            {
                _logger.LogError("CreateOrder: Course {CourseId} already purchased (pending or paid) by user {UserId}", request.CourseId, userId);
                return new ServiceResponse { Succeeded = false, Message = "Course already purchased (pending or paid)!" };
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