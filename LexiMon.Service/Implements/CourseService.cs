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

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CourseService> _logger;

    public CourseService(IUnitOfWork unitOfWork, ILogger<CourseService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResponseData<Guid>> CreateCourseAsync(
        CourseRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Course, Guid>();
        var course = request.ToCourse();
        
        await repo.AddAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new ResponseData<Guid>
        {
            Succeeded = true,
            Message = "Course created successfully",
            Data = course.Id
        };
    }

    public async Task<ServiceResponse> UpdateCourseAsync(
        Guid id, CourseRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Course, Guid>();
        var course = repo.Query()
            .FirstOrDefault(c => c.Id == id && c.Status);
        if (course == null)
        {
            _logger.LogWarning("Course with id: {id} not found", id);
            return new ServiceResponse
            {
                Succeeded = false,
                Message = $"Course with id {id} not found"
            };
        }
        
        course.UpdateCourse(request);
        await repo.UpdateAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Course with id: {id} updated successfully!", id);
        return new ServiceResponse
        {
            Succeeded = true,
            Message = "Course updated successfully!"
        };
}

    public async Task<ServiceResponse> DeleteCourseAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Course, Guid>();
        var course = await repo.GetByIdAsync(id, cancellationToken);
        if (course == null)
        {
            _logger.LogWarning("Course with id: {id} not found", id);
            return new ServiceResponse
            {
                Succeeded = false,
                Message = $"Course with id {id} not found"
            };
        }
        
        await repo.RemoveAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Course with id: {id} deleted successfully", id);
        return new ServiceResponse
        {
            Succeeded = true,
            Message = "Course deleted successfully"
        };
    }

    public async Task<ResponseData<CourseResponseDto>> GetCourseByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Course, Guid>();
        var course = await repo.GetByIdAsync(id, cancellationToken);
        if (course == null)
        {
            
            _logger.LogWarning("Course with id: {id} not found", id);
            return new ResponseData<CourseResponseDto>
            {
                Succeeded = false,
                Message = $"Course with id {id} not found"
            };
        }

        var response = course.ToCourseResponse();
        
        _logger.LogInformation("Course with id: {id} retrieved successfully", id);
        return new ResponseData<CourseResponseDto>
        {
            Succeeded = true,
            Message = "Course retrieved successfully",
            Data = response
        };
}

    public async Task<PaginatedResponse<CourseResponseDto>> GetCoursesAsync(
        GetCourseRequest request,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Course, Guid>();

        var query = repo.Query() // 
            .AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            query = query.Where(c => c.Title.Contains(request.Title));
        }
        if(request.MinPrice != null && request.MaxPrice != null)
        {
            query = query.Where(c => (c.Price >= request.MinPrice && c.Price <= request.MaxPrice) || (c.Coin >= request.MinPrice && c.Coin <= request.MaxPrice));
        }
        else if(request.MinPrice != null)
        {
            query = query.Where(c => c.Price >= request.MinPrice || c.Coin >= request.MinPrice);
        }
        else if(request.MaxPrice != null)
        {
            query = query.Where(c => c.Price <= request.MaxPrice || c.Coin <= request.MaxPrice);
        }
        
        var totalCourses = query.Count();
        var coursesResponse = await query
            .OrderBy(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => c.ToCourseResponse())
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Course retrieved successfully");
        return new PaginatedResponse<CourseResponseDto>()
        {
            Succeeded = true,
            Message = "Courses retrieved successfully",
            TotalCount = totalCourses,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCourses / request.PageSize),
            Data = coursesResponse
        };
  
    }
}