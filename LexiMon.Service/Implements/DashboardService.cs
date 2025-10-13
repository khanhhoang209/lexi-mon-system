using LexiMon.Repository.Constants;
using LexiMon.Repository.Domains;
using LexiMon.Repository.Enum;
using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LexiMon.Service.Implements;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, ILogger<DashboardService> logger)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ServiceResponse> GetRevenueAsync(
        RevenueRequestDto requestBody,
        CancellationToken cancellationToken = default)
    {
        if (requestBody.StartDate > requestBody.EndDate)
        {
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Ngày bắt đầu phải trước ngày kết thúc!",
            };
        }

        var orders = await _unitOfWork.GetRepository<Order, Guid>()
            .Query()
            .Include(o => o.Course)
            .Include(o => o.Item).ThenInclude(o => o.Category)
            .AsNoTracking()
            .Where(o => o.PurchaseCost != null &&
                        o.PurchaseCost > 0 &&
                        o.PaymentStatus == PaymentStatus.Return &&
                        o.PaidAt >= requestBody.StartDate &&
                        o.PaidAt <= requestBody.EndDate)
            .ToListAsync(cancellationToken);

        var totalItemRevenue = orders
            .Where(o => o.ItemId != null &&
                        o.Item?.Category?.Name != Categories.PremiumPackage)
            .Sum(o => o.PurchaseCost ?? 0);

        var totalCourseRevenue = orders
            .Where(o => o.CourseId != null)
            .Sum(o => o.PurchaseCost ?? 0);

        var totalPremiumRevenue = orders
            .Where(o => o.ItemId != null &&
                        o.Item?.Category?.Name == Categories.PremiumPackage)
            .Sum(o => o.PurchaseCost ?? 0);


        var response = new RevenueResponseDto()
        {
            ItemRevenue = totalItemRevenue,
            CourseRevenue = totalCourseRevenue,
            PremiumRevenue = totalPremiumRevenue,
        };

        return new ResponseData<RevenueResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy doanh thu thành công!",
            Data = response,
        };
    }
}