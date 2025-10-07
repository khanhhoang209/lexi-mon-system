using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface ILevelRangeService
{
    Task<ResponseData<Guid>> CreateLevelRangeAsync(
        LevelRangeRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> UpdateLevelRangeAsync(
        Guid id,
        LevelRangeRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> DeleteLevelRangeAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<LevelRangeResponseDto>> GetLevelRangeByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedResponse<LevelRangeResponseDto>> GetLevelRangesAsync(
        GetBaseRequest request,
        CancellationToken cancellationToken = default);
}