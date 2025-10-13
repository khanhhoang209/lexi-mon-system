using LexiMon.Repository.Domains;
using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LexiMon.Service.Implements;

public class CourseLanguageService : ICourseLanguageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CourseLanguageService> _logger;

    public CourseLanguageService(IUnitOfWork unitOfWork, ILogger<CourseLanguageService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResponseData<List<CourseLanguageResponseDto>>> GetCourseLanguages(CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.GetRepository<CourseLanguage, Guid>();
        var courseLanguage = await repo.Query().Where(cl => cl.Status).ToListAsync(cancellationToken);
        var response = courseLanguage.Select(cl => new CourseLanguageResponseDto
        {
            Id = cl.Id,
            Name = cl.Name,
        }).ToList();
        return new ResponseData<List<CourseLanguageResponseDto>>()
        {
            Succeeded = true,
            Message = "Course languages retrieved",
            Data = response
        };
    }
}