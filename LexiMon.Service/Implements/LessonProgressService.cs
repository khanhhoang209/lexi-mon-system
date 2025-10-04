using LexiMon.Repository.Domains;
using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Mappers;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LexiMon.Service.Implements;

public class LessonProgressService : ILessonProgressService
{
    private IUnitOfWork _unitOfWork;
    private ILogger<LessonProgressService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public LessonProgressService(IUnitOfWork unitOfWork, ILogger<LessonProgressService> logger, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<PaginatedResponse<LessonProgressResponseDto>> GetLessonProgressAsync(
        string userId, 
        GetLessonProgressRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User not found!");
            return new PaginatedResponse<LessonProgressResponseDto>
                { 
                    Succeeded = false, 
                    Message = "User not found!" 
                };
        }
        
        var repo = _unitOfWork.GetRepository<LessonProgress, Guid>();
        var query = repo.Query()
            .Include(lp => lp.Lesson)
            .ThenInclude(l => l!.Course)
            .Include(lp => lp.CustomLesson)
            .Where(lp => lp.UserId == userId);

        if (request.LessonProgressStatus.HasValue)
        {
            query = query.Where(lp => lp.LessonProgressStatus == request.LessonProgressStatus);
        }
        if (request.TargetValue > 0)
            query = query.Where(lp => lp.TargetValue == request.TargetValue);

        if (request.CurrentValue > 0)
            query = query.Where(lp => lp.CurrentValue == request.CurrentValue);

        if (!string.IsNullOrWhiteSpace(request.LessonName))
            query = query.Where(lp => lp.Lesson!.Title.Contains(request.LessonName));

        if (!string.IsNullOrWhiteSpace(request.CustomLessonTitle))
            query = query.Where(lp => lp.CustomLesson!.Title.Contains(request.CustomLessonTitle));

        var totalCount = query.Count();
        
        var response = await query
            .OrderByDescending(lp => lp.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(lp => lp.ToLessonProgressResponse())
            .ToListAsync(cancellationToken);
        
        return new PaginatedResponse<LessonProgressResponseDto>()
        {
            Succeeded = true,
            Message = "Lesson Progress retrieved successfully.",
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            Data = response
        };
            
    }

    public async Task<ResponseData<LessonProgressResponseDto>> GetLessonProgressByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<LessonProgress, Guid>();
        var lessonProgress =  await repo.Query()
            .Include(lp => lp.Lesson)
            .ThenInclude(l => l!.Course)
            .Include(lp => lp.CustomLesson)
            .FirstOrDefaultAsync(lp => lp.Id == id);
        if (lessonProgress == null)
        {
            _logger.LogWarning("No lesson progress with id {id}", id);
            return new ResponseData<LessonProgressResponseDto>()
            {
                Succeeded = false,
                Message = "Not Found"
            };
        }

        var response = lessonProgress.ToLessonProgressResponse();
        _logger.LogInformation("Get lesson progress with id {id}", id);
        return new ResponseData<LessonProgressResponseDto>()
        {
            Succeeded = true,
            Message = "Lesson Progress retrieved successfully",
            Data = response
        };
    }

    public async Task<ResponseData<Guid>> CreateLessonProgressAsync(
        string userId,
        LessonProgressRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User not found!");
            return new ResponseData<Guid> { Succeeded = false, Message = "User not found!" };
        }
        
        var repo = _unitOfWork.GetRepository<LessonProgress, Guid>();
        var lessonProgress = request.ToLessonProgress(userId);
        await repo.AddAsync(lessonProgress, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Created lesson progress with id {id}", lessonProgress.Id);
        return new ResponseData<Guid>()
        {
            Succeeded = true,
            Message = "Create Lesson Progress Success!",
            Data = lessonProgress.Id
        };
    }

    public async Task<ServiceResponse> UpdateLessonProgressAsync(
        Guid id, 
        LessonProgressRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<LessonProgress, Guid>();
        var lessonProgress =  repo.Query().FirstOrDefault(lp => lp.Id == id);
        if (lessonProgress == null)
        {
            _logger.LogWarning("No lesson progress with id {id}", id);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Not Found"
            };
        }
        
        lessonProgress.UpdateLessonProgress(request);
        await repo.UpdateAsync(lessonProgress, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Updated lesson progress with id {id}", lessonProgress.Id);
        
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = "Update Lesson Progress Success!"
        };
    }

    public async Task<ServiceResponse> DeleteLessonProgressAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<LessonProgress, Guid>();
        var lessonProgress = repo.Query().FirstOrDefault(lp => lp.Id == id);
        if (lessonProgress == null)
        {
            _logger.LogWarning("No lesson progress with id {id}", id);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Not Found"
            };
        }

        await repo.RemoveAsync(lessonProgress, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Delete success!!");
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = "Delete Lesson Progress Success!"
        };
    }

    public async Task<PaginatedResponse<LessonProgressResponseDto>> GetLessonProgressByLessonIdAsync(
        string userId, 
        Guid lessonId, 
        GetLessonProgressByLessonIdRequest request,
        CancellationToken cancellationToken = default)
    {
     var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User not found!");
            return new PaginatedResponse<LessonProgressResponseDto>
                { 
                    Succeeded = false, 
                    Message = "User not found!" 
                };
        }
        
        var repo = _unitOfWork.GetRepository<LessonProgress, Guid>();
        var query = repo.Query()
            .Include(lp => lp.Lesson)
            .ThenInclude(l => l!.Course)
            .Include(lp => lp.CustomLesson)
            .Where(lp => lp.UserId == userId && lp.LessonId == lessonId);

        if (request.LessonProgressStatus.HasValue)
        {
            query = query.Where(lp => lp.LessonProgressStatus == request.LessonProgressStatus);
        }
        if (request.TargetValue > 0)
            query = query.Where(lp => lp.TargetValue == request.TargetValue);

        if (request.CurrentValue > 0)
            query = query.Where(lp => lp.CurrentValue == request.CurrentValue);

        if (!string.IsNullOrWhiteSpace(request.Title))
            query = query.Where(lp => lp.Lesson!.Title.Contains(request.Title));

        var totalCount = query.Count();
        
        var response = await query
            .OrderByDescending(lp => lp.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(lp => lp.ToLessonProgressResponse())
            .ToListAsync(cancellationToken);
        
        return new PaginatedResponse<LessonProgressResponseDto>()
        {
            Succeeded = true,
            Message = "Lesson Progress retrieved successfully.",
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            Data = response
        };
    }

    public async Task<PaginatedResponse<LessonProgressResponseDto>> GetLessonProgressByCustomLessonAsync(
        string userId, 
        Guid customLessonId,
        GetLessonProgressByLessonIdRequest request, 
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User not found!");
            return new PaginatedResponse<LessonProgressResponseDto>
            { 
                Succeeded = false, 
                Message = "User not found!" 
            };
        }
        
        var repo = _unitOfWork.GetRepository<LessonProgress, Guid>();
        var query = repo.Query()
            .Include(lp => lp.Lesson)
            .ThenInclude(l => l!.Course)
            .Include(lp => lp.CustomLesson)
            .Where(lp => lp.UserId == userId && lp.CustomLessonId == customLessonId);

        if (request.LessonProgressStatus.HasValue)
        {
            query = query.Where(lp => lp.LessonProgressStatus == request.LessonProgressStatus);
        }
        if (request.TargetValue > 0)
            query = query.Where(lp => lp.TargetValue == request.TargetValue);

        if (request.CurrentValue > 0)
            query = query.Where(lp => lp.CurrentValue == request.CurrentValue);

        if (!string.IsNullOrWhiteSpace(request.Title))
            query = query.Where(lp => lp.CustomLesson!.Title.Contains(request.Title));

        var totalCount = query.Count();
        
        var response = await query
            .OrderByDescending(lp => lp.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(lp => lp.ToLessonProgressResponse())
            .ToListAsync(cancellationToken);
        
        return new PaginatedResponse<LessonProgressResponseDto>()
        {
            Succeeded = true,
            Message = "Lesson Progress retrieved successfully.",
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            Data = response
        };
    }
}