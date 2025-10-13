using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;

namespace LexiMon.Service.Interfaces;

public interface IDashboardService
{
    Task<ServiceResponse> GetRevenueAsync(
        RevenueRequestDto requestBody,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse> GetTotalUsersAsync(CancellationToken cancellationToken = default);

    Task<ServiceResponse> GetTotalCoursesAsync(CancellationToken cancellationToken = default);

    Task<ServiceResponse> GetTotalItemsAsync(CancellationToken cancellationToken = default);

    Task<ServiceResponse> GetTotalCoinsAsync(CancellationToken cancellationToken = default);

    Task<ServiceResponse> GetSubscriptionAsync(CancellationToken cancellationToken = default);

    Task<ServiceResponse> GetPopularLanguagesAsync(
        PopularLanguageRequestDto requestBody,
        CancellationToken cancellationToken = default);

}