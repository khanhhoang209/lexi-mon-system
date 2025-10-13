using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface ICourseLanguageService
{
    Task<ResponseData<List<CourseLanguageResponseDto>>> GetCourseLanguages(CancellationToken cancellationToken);
}