using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface IEnemyLevelService
{
    Task<ResponseData<Guid>> CreateEnemyLevelAsync(
        EnemyLevelRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> UpdateEnemyLevelAsync(
        Guid id,
        EnemyLevelRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> DeleteEnemyLevelAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<EnemyLevelResponseDto>> GetEnemyLevelByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedResponse<EnemyLevelResponseDto>> GetEnemyLevelsAsync(
        GetBaseRequest request,
        CancellationToken cancellationToken = default);
}