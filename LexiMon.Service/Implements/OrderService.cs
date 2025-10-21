using LexiMon.Repository.Constants;
using LexiMon.Repository.Domains;
using LexiMon.Repository.Enum;
using LexiMon.Repository.Interfaces;
using LexiMon.Repository.Utils;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using LexiMon.Service.Mappers;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;
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

            var validationResponse = await ValidateAndResolveTargetAsync(request, user, cancellationToken);
            if (!validationResponse.Succeeded)
            {
                return validationResponse;
            }

            // Lấy giá
            var pricing = await ResolvePricingAsync(request, cancellationToken);
            if (pricing is null)
                return new ServiceResponse { Succeeded = false, Message = "Pricing not found!" };

            if (pricing.CoinCost is > 0)
            {
                if (user.Coins < pricing.CoinCost)
                {
                    return new ServiceResponse
                    {
                        Succeeded = false,
                        Message = "Không đủ xu để thực hiện giao dịch!"
                    };
                }

                var priceOrder = request.ToOrder(userId, pricing.PurchaseCost, pricing.CoinCost);
                priceOrder.PaymentStatus = PaymentStatus.Return;
                priceOrder.PaidAt = TimeConverter.GetCurrentVietNamTime();
                user.Coins -= (decimal) pricing.CoinCost;
                await _userManager.UpdateAsync(user);
                await _unitOfWork.GetRepository<Order, Guid>().AddAsync(priceOrder, cancellationToken);
                
                // Update UserDeck if course is purchased
                if (priceOrder.Course != null)
                {
                    var userDeck = new UserDeck()
                    {
                        UserId = priceOrder.UserId,
                        CourseId = priceOrder.CourseId,
                    };
                    await _unitOfWork.GetRepository<UserDeck, Guid>().AddAsync(userDeck, cancellationToken);
                }

                // Update Equipment if item is purchased
                if (priceOrder.Item != null)
                {
                    var character = await _unitOfWork.GetRepository<Character, Guid>()
                        .Query()
                        .FirstOrDefaultAsync(c => c.UserId == priceOrder.UserId, cancellationToken);

                    var equipment = new Equipment()
                    {
                        CharacterId = character!.Id,
                        ItemId = (Guid)priceOrder.ItemId!,
                    };
                    await _unitOfWork.GetRepository<Equipment, (Guid, Guid)>().AddAsync(equipment, cancellationToken);
                }
                
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Order created successfully with Coin payment with OrderId: {OrderId}", priceOrder.Id);
                return new ResponseData<Guid>()
                {
                    Succeeded = true,
                    Message = "Order created successfully with Coin payment.",
                    Data = priceOrder.Id
                };
            }

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
            .Include(o => o.Item).ThenInclude(o => o!.Category)
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
                if (Categories.PremiumPackage == order.Item.Category!.Name)
                {
                    var user = await _userManager.FindByIdAsync(order.UserId);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (!roles.Contains(Roles.Premium))
                        {
                            await _userManager.RemoveFromRoleAsync(user, Roles.Free);
                            await _userManager.AddToRoleAsync(user, Roles.Premium);
                            user.PremiumUntil = TimeConverter.GetCurrentVietNamTime().AddMonths(1);
                            await _userManager.UpdateAsync(user);
                            _logger.LogInformation("User {UserId} upgraded to Premium role after purchasing Premium Package", user.Id);
                        }
                    }
                }
                else
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

    public async Task<PaginatedResponse<OrderUserResponseDto>> GetAllUsersOrdersByUserId(
        GetOrderUserRequest request,
        string userId,
        CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var query = repo
            .Query().Where(o => o.UserId == userId);
        query = query
            .Include(o => o.Course)
            .Include(o => o.Item);

        if (request.MinPrice != null && request.MaxPrice != null)
        {
            query = query.Where(o => (o.PurchaseCost >= request.MinPrice && o.PurchaseCost <= request.MaxPrice)
                                     || (o.CoinCost >= request.MinPrice && o.CoinCost <= request.MaxPrice));
        }
        else if (request.MinPrice != null)
        {
            query = query.Where(o => o.PurchaseCost >= request.MinPrice || o.CoinCost >= request.MinPrice);
        }
        else if (request.MaxPrice != null)
        {
            query = query.Where(o => o.PurchaseCost <= request.MaxPrice || o.CoinCost <= request.MaxPrice);
        }

        if (request.PaymentStatus.HasValue)
            query = query.Where(o => o.PaymentStatus == request.PaymentStatus);

        if (!string.IsNullOrEmpty(request.Name))
            query = query.Where(o => (o.Item != null && o.Item.Name.Contains(request.Name))
                                     || (o.Course != null && o.Course.Title.Contains(request.Name)));
        if (request.FromDate.HasValue && request.ToDate.HasValue)
            query = query.Where(o => o.CreatedAt >= request.FromDate && o.CreatedAt <= request.ToDate);
        else if (request.FromDate.HasValue)
            query = query.Where(o => o.CreatedAt >= request.FromDate);
        else if (request.ToDate.HasValue)
            query = query.Where(o => o.CreatedAt <= request.ToDate);
        
        if(request.IsActive.HasValue)
            query = query.Where(a => a.Status == request.IsActive.Value);
        var totalCount = query.Count();
        var response = await query
            .OrderByDescending(o => o.Status)
            .ThenByDescending(o => o.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => o.ToOrderUserResponse())
            .ToListAsync(cancellationToken);
        return new PaginatedResponse<OrderUserResponseDto>
        {
            Succeeded = true,
            Message = "Get orders successfully",
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            Data = response
        };
    }
    
    public async Task<PaginatedResponse<OrderResponseDto>> GetAllOrders(
        GetOrderRequest request,
        CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var query = repo.Query();
            query = query.Include(o => o.Course)
            .Include(o => o.Item)
            .Include(o => o.User);

        if (request.MinPrice != null && request.MaxPrice != null)
        {
            query = query.Where(o => (o.PurchaseCost >= request.MinPrice && o.PurchaseCost <= request.MaxPrice)
                                     || (o.CoinCost >= request.MinPrice && o.CoinCost <= request.MaxPrice));
        }
        else if (request.MinPrice != null)
        {
            query = query.Where(o => o.PurchaseCost >= request.MinPrice || o.CoinCost >= request.MinPrice);
        }
        else if (request.MaxPrice != null)
        {
            query = query.Where(o => o.PurchaseCost <= request.MaxPrice || o.CoinCost <= request.MaxPrice);
        }
        
        if (request.PaymentStatus.HasValue)
            query = query.Where(o => o.PaymentStatus == request.PaymentStatus);

        if (!string.IsNullOrEmpty(request.Name))
            query = query.Where(o => (o.Item != null && o.Item.Name.Contains(request.Name))
                                     || (o.Course != null && o.Course.Title.Contains(request.Name)));
        if (request.FromDate.HasValue && request.ToDate.HasValue)
            query = query.Where(o => o.CreatedAt >= request.FromDate && o.CreatedAt <= request.ToDate);
        else if (request.FromDate.HasValue)
            query = query.Where(o => o.CreatedAt >= request.FromDate);
        else if (request.ToDate.HasValue)
            query = query.Where(o => o.CreatedAt <= request.ToDate);
        
        if(!string.IsNullOrEmpty(request.OrderType))
        {
            if ("course".Equals(request.OrderType, StringComparison.InvariantCultureIgnoreCase))
            {
                query = query.Where(o => o.CourseId != null);
            }
            else if ("item".Equals(request.OrderType, StringComparison.InvariantCultureIgnoreCase))
            {
                query = query.Where(o => o.ItemId != null);
            }
        }
        if(!string.IsNullOrEmpty(request.Email))
        {
            query = query.Where(o =>o.User!.Email.Contains(request.Email));
        }
        var totalCount = query.Count();
        var response = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => o.ToOrderResponse())
            .ToListAsync(cancellationToken);
        return new PaginatedResponse<OrderResponseDto>
        {
            Succeeded = true,
            Message = "Get orders successfully",
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            Data = response
};
    }
    #region Helpers

    /// <summary>
    /// Kiểm tra rằng request chỉ chọn đúng 1 mục tiêu (Course hoặc Item)
    /// </summary>
    private async Task<ServiceResponse> ValidateAndResolveTargetAsync(
        OrderRequestDto request,
        ApplicationUser user,
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
            if (item.IsPremium)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Premium"))
                {
                    return new ServiceResponse
                    {
                        Succeeded = false,
                        Message = "Only users with the premium role can purchase premium items."
                    };
                }
            }
            var dupItem = await orderRepo.Query()
                .AnyAsync(o => o.UserId == user.Id
                               && o.ItemId == request.ItemId
                               && (o.PaymentStatus == PaymentStatus.Pending || o.PaymentStatus == PaymentStatus.Return)
                , ct);



            if (dupItem)
            {
                _logger.LogError("CreateOrder: Item {ItemId} already purchased (pending or paid) by user {UserId}", request.ItemId, user.Id);
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
                .AnyAsync(o => o.UserId == user.Id
                            && o.CourseId == request.CourseId
                            && (o.PaymentStatus == PaymentStatus.Pending || o.PaymentStatus == PaymentStatus.Return)
                    , ct);
            if (dupCourse)
            {
                _logger.LogError("CreateOrder: Course {CourseId} already purchased (pending or paid) by user {UserId}", request.CourseId, user.Id);
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