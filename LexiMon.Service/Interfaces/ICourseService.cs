using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface ICourseService
{
    Task<ResponseData<Guid>> CreateCourseAsync(
        CourseRequestDto requestDto,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> UpdateCourseAsync(
        Guid id,
        CourseRequestDto requestDto,
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