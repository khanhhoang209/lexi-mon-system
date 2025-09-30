using LexiMon.Repository.Domains;
using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Mappers;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LexiMon.Service.Implements;

public class LessonService : ILessonService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LessonService> _logger;

    public LessonService(IUnitOfWork unitOfWork, ILogger<LessonService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResponseData<Guid>> CreateLessonAsync(
        LessonRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        var courseRepo = _unitOfWork.GetRepository<Course, Guid>();
        var course = await courseRepo.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
        {
            _logger.LogWarning("Cannot create lesson!! Course with Id: {CourseId} not found", request.CourseId);
            return new ResponseData<Guid>()
            {
                Succeeded = false,
                Message = "Course not found! Cannot create lesson.",
                Data = Guid.Empty
            };
        }
        
        var lessonRepo = _unitOfWork.GetRepository<Lesson, Guid>();
        var lesson = request.ToLesson();
        
        await lessonRepo.AddAsync(lesson, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ResponseData<Guid>()
        {
            Succeeded = true,
            Message = "Lesson created successfully",
            Data = lesson.Id
        };
    }

    public async Task<ServiceResponse> UpdateLessonAsync(Guid id, LessonRequestDto request, CancellationToken cancellationToken = default)
    {
        var courseRepo = _unitOfWork.GetRepository<Course, Guid>();
        var course = await courseRepo.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
        {
            _logger.LogWarning("Cannot update lesson!! Course with Id: {CourseId} not found", request.CourseId);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Course not found! Cannot update lesson."
            };
        }
        
        var lessonRepo = _unitOfWork.GetRepository<Lesson, Guid>();
        var lesson = await lessonRepo.GetByIdAsync(id, cancellationToken);
        if (lesson == null)
        {
            _logger.LogWarning("Lesson with Id: {id} not found", id);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Lesson not found!"
            };
        }
        
        lesson.UpdateLesson(request);
        await lessonRepo.UpdateAsync(lesson, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ServiceResponse()
            {
                Succeeded = true,
                Message = "Lesson updated successfully"
            };
        
    }

    public async Task<ServiceResponse> DeleteLessonAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lessonRepo = _unitOfWork.GetRepository<Lesson, Guid>();
        var lesson = await lessonRepo.GetByIdAsync(id, cancellationToken);
        if (lesson == null)
        {
            _logger.LogWarning("Cannot delete lesson!! Lesson with Id: {CourseId} not found", id);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Lesson not found!"
            };
        }

        await lessonRepo.RemoveAsync(lesson, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ServiceResponse() 
        { 
            Succeeded = true, 
            Message = "Lesson deleted successfully"
        };
    }

    public async Task<ResponseData<LessonResponseDto>> GetLessonByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lessonRepo = _unitOfWork.GetRepository<Lesson, Guid>();
        var lessonResponse = await lessonRepo.Query()
            .Include(l => l.Course)
            .Where(l => l.Id == id)
            .Select(l => l.ToLessonResponse())
            .FirstOrDefaultAsync(cancellationToken);
        
        if (lessonResponse == null)
        {
            _logger.LogWarning("Cannot get lesson!! Lesson with Id: {id} not found", id);
            return new ResponseData<LessonResponseDto>()
            {
                Succeeded = false,
                Message = "Lesson not found!",
                Data = null
            };
        }
        
        return new ResponseData<LessonResponseDto>()
        {
            Succeeded = true,
            Message = "Lesson retrieved successfully",
            Data = lessonResponse
        };
    }

    public async Task<PaginatedResponse<LessonResponseDto>> GetLessonsByCourseIdAsync(
        Guid courseId, 
        GetLessonRequest request,
        CancellationToken cancellationToken = default)
    {
        var courseRepo = _unitOfWork.GetRepository<Course, Guid>();
        var course = await courseRepo.GetByIdAsync(courseId, cancellationToken);
        if (course == null)
        {
            _logger.LogWarning("Cannot get lesson!! Course with Id: {CourseId} not found", courseId);
            return new PaginatedResponse<LessonResponseDto>()
            {
                Succeeded = false,
                Message = "Course not found!"
            };
        }
        
        var lessonRepo = _unitOfWork.GetRepository<Lesson, Guid>();

        var query = lessonRepo.Query()
            .Where(l => l.CourseId == course.Id);

        if (!string.IsNullOrEmpty(request.Title))
        {
            query = query.Where(l => l.Title.Contains(request.Title));
        }

        var totalLesson = query.Count();
        
        var lessonResponse = await query
            .OrderByDescending(l => l.Title)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => l.ToLessonResponse())
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<LessonResponseDto>()
        {
            Succeeded = true,
            Message = "Lessons retrieved successfully",
            TotalCount = totalLesson,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalLesson / request.PageSize),
            Data = lessonResponse
        };
        
    }
}