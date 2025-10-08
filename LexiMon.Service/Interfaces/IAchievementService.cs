using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface IAchievementService
{
    Task<ResponseData<Guid>> CreateAchievementAsync(
        AchievementRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> UpdateAchievementAsync(
        Guid id,
        AchievementRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> DeleteAchievementAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<AchievementResponseDto>> GetAchievementByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedResponse<AchievementResponseDto>> GetAchievementsAsync(
        GetBaseRequest request,
        CancellationToken cancellationToken = default);
}