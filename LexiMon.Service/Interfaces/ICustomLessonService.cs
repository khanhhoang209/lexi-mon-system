using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface ICustomLessonService
{
    Task<ResponseData<Guid>> CreateCustomLessonAsync(
        CustomLessonRequestDto request,
        string userId,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<Guid>> UpdateCustomLessonAsync(
        Guid id,
        CustomLessonRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResponseData<Guid>> DeleteCustomLessonAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<CustomLessonResponseDto>> GetCustomLessonByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<List<CustomLessonResponseDto>>> GetCustomLessonsAsync(
        string userId,
        CancellationToken cancellationToken = default);
}