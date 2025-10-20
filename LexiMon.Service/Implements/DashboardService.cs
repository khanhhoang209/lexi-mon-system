using LexiMon.Repository.Constants;
using LexiMon.Repository.Domains;
using LexiMon.Repository.Enum;
using LexiMon.Repository.Interfaces;
using LexiMon.Repository.Utils;
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

        var response = await GetRevenueByDate(
            requestBody.StartDate ?? DateTimeOffset.MinValue,
            requestBody.EndDate ?? DateTimeOffset.MaxValue,
            cancellationToken);

        return new ResponseData<RevenueResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy doanh thu thành công!",
            Data = response,
        };
    }

    public async Task<ServiceResponse> GetTotalUsersAsync(CancellationToken cancellationToken = default)
    {
        const int admin = 1;
        var totalUsers = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Status)
            .CountAsync(cancellationToken);
        return new ResponseData<int>()
        {
            Succeeded = true,
            Message = "Lấy tổng số người dùng thành công!",
            Data = totalUsers - admin
        };
    }

    public async Task<ServiceResponse> GetTotalCoursesAsync(CancellationToken cancellationToken = default)
    {
        var totalCourses = await _unitOfWork.GetRepository<Course, Guid>()
            .Query()
            .AsNoTracking()
            .Where(c => c.Status)
            .CountAsync(cancellationToken);
        return new ResponseData<int>()
        {
            Succeeded = true,
            Message = "Lấy tổng số khóa học thành công!",
            Data = totalCourses
        };
    }

    public async Task<ServiceResponse> GetTotalItemsAsync(CancellationToken cancellationToken = default)
    {
        var totalItems = _unitOfWork.GetRepository<Item, Guid>()
            .Query()
            .Include(i => i.Category)
            .AsNoTracking()
            .Where(i => i.Status &&
                        i.Category != null &&
                        i.Category.Name != Categories.PremiumPackage)
            .CountAsync(cancellationToken);
        return new ResponseData<int>()
        {
            Succeeded = true,
            Message = "Lấy tổng số vật phẩm thành công!",
            Data = await totalItems
        };
    }

    public async Task<ServiceResponse> GetTotalCoinsAsync(CancellationToken cancellationToken = default)
    {
        var itemCoin = await _unitOfWork.GetRepository<Order, Guid>()
            .Query()
            .AsNoTracking()
            .Include(o => o.Item)
            .ThenInclude(o => o!.Category)
            .Where(o => o.CoinCost != null &&
                        o.CoinCost > 0 &&
                        o.Item != null &&
                        o.Item.Category != null &&
                        o.Item.Category.Name != Categories.PremiumPackage &&
                        o.PaymentStatus == PaymentStatus.Return)
            .SumAsync(o => o.CoinCost ?? 0, cancellationToken);

        var courseCoin = await _unitOfWork.GetRepository<Order, Guid>()
            .Query()
            .AsNoTracking()
            .Include(o => o.Course)
            .Where(o => o.CoinCost != null &&
                        o.CoinCost > 0 &&
                        o.Course != null &&
                        o.PaymentStatus == PaymentStatus.Return)
            .SumAsync(o => o.CoinCost ?? 0, cancellationToken);

        var response = new CoinResponseDto()
        {
            ItemCoin = itemCoin,
            CourseCoin = courseCoin,
        };

        return new ResponseData<CoinResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy tổng số xu thành công!",
            Data = response
        };
    }

    public async Task<ServiceResponse> GetSubscriptionAsync(CancellationToken cancellationToken = default)
    {
        var userFree = await _userManager.GetUsersInRoleAsync(Roles.Free);
        var userPremium = await _userManager.GetUsersInRoleAsync(Roles.Premium);

        var response = new SubscriptionResponseDto()
        {
            Free = userFree.Count,
            Premium = userPremium.Count,
        };

        return new ResponseData<SubscriptionResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy tổng số người dùng theo gói thành công!",
            Data = response,
        };
    }

    public async Task<ServiceResponse> GetPopularLanguagesAsync(PopularLanguageRequestDto requestBody,
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

        const int admin = 1;
        var totalUsers = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Status)
            .CountAsync(cancellationToken) - admin;

        var english = await CalculateLanguage(requestBody, Languages.English, totalUsers, cancellationToken);
        var japanese = await CalculateLanguage(requestBody, Languages.Japanese, totalUsers, cancellationToken);
        var korean = await CalculateLanguage(requestBody, Languages.Korean, totalUsers, cancellationToken);
        var chinese = await CalculateLanguage(requestBody, Languages.Chinese, totalUsers, cancellationToken);

        var response = new PopularLanguageResponseDto()
        {
            English = english,
            Japanese = japanese,
            Korean = korean,
            Chinese = chinese,
        };

        return new ResponseData<PopularLanguageResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy ngôn ngữ phổ biến thành công!",
            Data = response,
        };
    }

    public async Task<ServiceResponse> GetWeeklyRevenueAsync(CancellationToken cancellationToken = default)
    {
        var startOfWeek = GetStartOfWeek(TimeConverter.GetCurrentVietNamTime());

        var monday = await GetDailyRevenueAsync(startOfWeek, cancellationToken);
        var tuesday = await GetDailyRevenueAsync(startOfWeek.AddDays(1), cancellationToken);
        var wednesday = await GetDailyRevenueAsync(startOfWeek.AddDays(2), cancellationToken);
        var thursday = await GetDailyRevenueAsync(startOfWeek.AddDays(3), cancellationToken);
        var friday = await GetDailyRevenueAsync(startOfWeek.AddDays(4), cancellationToken);
        var saturday = await GetDailyRevenueAsync(startOfWeek.AddDays(5), cancellationToken);
        var sunday = await GetDailyRevenueAsync(startOfWeek.AddDays(6), cancellationToken);

        var response = new WeeklyRevenueResponseDto()
        {
            Monday = monday,
            Tuesday = tuesday,
            Wednesday = wednesday,
            Thursday = thursday,
            Friday = friday,
            Saturday = saturday,
            Sunday = sunday,
        };

        return new ResponseData<WeeklyRevenueResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy doanh thu tuần thành công!",
            Data = response,
        };
    }

    public async Task<ServiceResponse> GetMonthlyRevenueAsync(CancellationToken cancellationToken = default)
    {
        var startOfMonth = GetStartOfMonth(TimeConverter.GetCurrentVietNamTime());
        var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

        var response = await GetRevenueByDate(startOfMonth, endOfMonth, cancellationToken);

        return new ResponseData<RevenueResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy doanh thu tháng thành công!",
            Data = response,
        };
    }

    public async Task<ServiceResponse> GetYearlyRevenueAsync(CancellationToken cancellationToken = default)
    {
        var startOfYear = GetStartOfYear(TimeConverter.GetCurrentVietNamTime());
        var january = await GetRevenueByDate(
            startOfYear, startOfYear.AddMonths(1).AddTicks(-1), cancellationToken);
        var february = await GetRevenueByDate(
            startOfYear.AddMonths(1), startOfYear.AddMonths(2).AddTicks(-1), cancellationToken);
        var march = await GetRevenueByDate(
            startOfYear.AddMonths(2), startOfYear.AddMonths(3).AddTicks(-1), cancellationToken);
        var april = await GetRevenueByDate(
            startOfYear.AddMonths(3), startOfYear.AddMonths(4).AddTicks(-1), cancellationToken);
        var may = await GetRevenueByDate(
            startOfYear.AddMonths(4), startOfYear.AddMonths(5).AddTicks(-1), cancellationToken);
        var june = await GetRevenueByDate(
            startOfYear.AddMonths(5), startOfYear.AddMonths(6).AddTicks(-1), cancellationToken);
        var july = await GetRevenueByDate(
            startOfYear.AddMonths(6), startOfYear.AddMonths(7).AddTicks(-1), cancellationToken);
        var august = await GetRevenueByDate(
            startOfYear.AddMonths(7), startOfYear.AddMonths(8).AddTicks(-1), cancellationToken);
        var september = await GetRevenueByDate(
            startOfYear.AddMonths(8), startOfYear.AddMonths(9).AddTicks(-1), cancellationToken);
        var october = await GetRevenueByDate(
            startOfYear.AddMonths(9), startOfYear.AddMonths(10).AddTicks(-1), cancellationToken);
        var november = await GetRevenueByDate(
            startOfYear.AddMonths(10), startOfYear.AddMonths(11).AddTicks(-1), cancellationToken);
        var december = await GetRevenueByDate(
            startOfYear.AddMonths(11), startOfYear.AddMonths(12).AddTicks(-1), cancellationToken);

        var response = new MonthlyRevenueResponseDto()
        {
            January = january,
            February = february,
            March = march,
            April = april,
            May = may,
            June = june,
            July = july,
            August = august,
            September = september,
            October = october,
            November = november,
            December = december,
        };

        return new ResponseData<MonthlyRevenueResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy doanh thu năm thành công!",
            Data = response,
        };
    }


    #region Method helper

        private async Task<int> CalculateLanguage(
            PopularLanguageRequestDto requestBody,
            string language,
            int totalUsers,
            CancellationToken cancellationToken = default)
        {
            if (totalUsers == 0) return 0;
            var count = await _unitOfWork.GetRepository<UserDeck, Guid>()
                .Query()
                .AsNoTracking()
                .Include(d => d.Course).ThenInclude(d => d!.CourseLanguage)
                .Where(d => d.Status &&
                            d.Course != null &&
                            d.CreatedAt >= requestBody.StartDate &&
                            d.CreatedAt <= requestBody.EndDate &&
                            d.Course.CourseLanguage != null &&
                            d.Course.CourseLanguage.Name == language)
                .CountAsync(cancellationToken);
            return (int)Math.Round((double)count / totalUsers * 100);
        }

        private DateTimeOffset GetStartOfWeek(DateTimeOffset currentDate)
        {
            var delta = DayOfWeek.Monday - currentDate.DayOfWeek;
            return currentDate.AddDays(delta).Date;
        }

        private DateTimeOffset GetStartOfMonth(DateTimeOffset currentDate)
        {
            return new DateTimeOffset(currentDate.Year, currentDate.Month, 1, 0, 0, 0, currentDate.Offset);
        }

        private DateTimeOffset GetStartOfYear(DateTimeOffset currentDate)
        {
            return new DateTimeOffset(currentDate.Year, 1, 1, 0, 0, 0, currentDate.Offset);
        }

        private async Task<RevenueResponseDto> GetDailyRevenueAsync(DateTimeOffset date, CancellationToken cancellationToken = default)
        {
            var orders = await _unitOfWork.GetRepository<Order, Guid>()
                .Query()
                .Include(o => o.Course)
                .Include(o => o.Item).ThenInclude(o => o!.Category)
                .AsNoTracking()
                .Where(o => o.PurchaseCost != null &&
                            o.PurchaseCost > 0 &&
                            o.PaymentStatus == PaymentStatus.Return &&
                            o.PaidAt >= date &&
                            o.PaidAt <= date.AddDays(1).AddTicks(-1))
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

            return new RevenueResponseDto()
            {
                ItemRevenue = totalItemRevenue,
                CourseRevenue = totalCourseRevenue,
                PremiumRevenue = totalPremiumRevenue,
            };
        }

        private async Task<RevenueResponseDto> GetRevenueByDate(
            DateTimeOffset fromDate,
            DateTimeOffset toDate,
            CancellationToken cancellationToken = default)
        {
            var orders = await _unitOfWork.GetRepository<Order, Guid>()
                .Query()
                .Include(o => o.Course)
                .Include(o => o.Item).ThenInclude(o => o.Category)
                .AsNoTracking()
                .Where(o => o.PurchaseCost != null &&
                            o.PurchaseCost > 0 &&
                            o.PaymentStatus == PaymentStatus.Return &&
                            o.PaidAt >= fromDate &&
                            o.PaidAt <= toDate.AddDays(1).AddTicks(-1))
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

            return new RevenueResponseDto()
            {
                ItemRevenue = totalItemRevenue,
                CourseRevenue = totalCourseRevenue,
                PremiumRevenue = totalPremiumRevenue,
            };
        }

    #endregion
}