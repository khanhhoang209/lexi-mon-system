using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface ICourseService
{
    Task<ResponseData<Guid>> CreateCourseAsync(
        CourseRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> UpdateCourseAsync(
        Guid id,
        CourseRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> DeleteCourseAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<CourseResponseDto>> GetCourseByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedResponse<CourseResponseDto>> GetCoursesAsync(
        GetCourseRequest request,
        CancellationToken cancellationToken = default);
}