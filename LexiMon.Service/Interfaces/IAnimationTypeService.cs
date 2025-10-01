using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface IAnimationTypeService
{
    Task<ResponseData<Guid>> CreateAnimationTypeAsync(
        AnimationTypeRequestDto request,
        CancellationToken cancellationToken = default);
    Task<ServiceResponse> UpdateAnimationTypeAsync(
        Guid id,
        AnimationTypeRequestDto request,
        CancellationToken cancellationToken = default);
    Task<ServiceResponse> DeleteAnimationTypeAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<ResponseData<AnimationTypeResponseDto>> GetAnimationTypeById(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<PaginatedResponse<AnimationTypeResponseDto>> GetAllAnimationTypesAsync(
        GetAnimationTypeRequest request,
        CancellationToken cancellationToken = default);
}