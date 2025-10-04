using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface ILessonProgressService
{
    Task<PaginatedResponse<LessonProgressResponseDto>> GetLessonProgressAsync(
        string userId,
        GetLessonProgressRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<LessonProgressResponseDto>> GetLessonProgressByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<Guid>> CreateLessonProgressAsync(
        string userId,
        LessonProgressRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> UpdateLessonProgressAsync(
        Guid id, 
        LessonProgressRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> DeleteLessonProgressAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}