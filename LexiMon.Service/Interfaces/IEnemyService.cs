using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface IEnemyService
{
    Task<ResponseData<Guid>> CreateEnemyAsync(
        EnemyRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> UpdateEnemyAsync(
        Guid id,
        EnemyRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> DeleteEnemyAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<EnemyResponseDto>> GetEnemyByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedResponse<EnemyResponseDto>> GetEnemiesAsync(
        GetBaseRequest request,
        CancellationToken cancellationToken = default);
}