using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface ILessonService
{
    Task<ResponseData<Guid>> CreateLessonAsync(
        LessonRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> UpdateLessonAsync(
        Guid id,
        LessonRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> DeleteLessonAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<LessonResponseDto>> GetLessonByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedResponse<LessonResponseDto>> GetLessonsByCourseIdAsync(
        Guid courseId,
        GetLessonRequest request,
        CancellationToken cancellationToken = default);
}