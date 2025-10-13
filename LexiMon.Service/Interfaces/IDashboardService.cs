using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;

namespace LexiMon.Service.Interfaces;

public interface IDashboardService
{
    Task<ServiceResponse> GetRevenueAsync(
        RevenueRequestDto requestBody,
        CancellationToken cancellationToken = default);
}