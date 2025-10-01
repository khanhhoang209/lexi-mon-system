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

public class UserDeckService : IUserDeckService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    ILogger<UserDeckService> _logger;

    public UserDeckService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, ILogger<UserDeckService> logger)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PaginatedResponse<UserDeckResponseDto>> GetUserDecksByUserIdAsync(
        string userId, 
        GetUserDeckRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userDeckRepo = _unitOfWork.GetRepository<UserDeck, Guid>();
            var query =  userDeckRepo.Query(true)
                .Include(ud => ud.Course)
                .Include(ud => ud.CustomLesson)
                .Where(ud => ud.UserId == userId && ud.Status)
                .AsNoTracking();
        
            if (!string.IsNullOrEmpty(request.CourseTitle))
                query = query.Where(ud => ud.Course!.Title.Contains(request.CourseTitle));
        
            if(!string.IsNullOrEmpty(request.CustomLessonTitle))
                query = query.Where(ud => ud.CustomLesson!.Title.Contains(request.CustomLessonTitle));
        
            var totalCourses = query.Count();
            var userDeckResponse = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(ud => ud.ToUserDeckResponse())
                .ToListAsync(cancellationToken);
        
            return new PaginatedResponse<UserDeckResponseDto>()
            {
                Succeeded = true,
                Message = "User decks retrieved successfully.",
                TotalCount = totalCourses,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCourses / request.PageSize),
                Data = userDeckResponse
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new PaginatedResponse<UserDeckResponseDto>()
            {
                Succeeded = false,
                Message = "Fail to get user deck!! Error: " + e.Message
            };
        }
     
    }

    public async Task<ResponseData<UserDeckResponseDto>> GetUserDeckByIdAsync(
        Guid userDeckId, 
        CancellationToken cancellationToken = default)
    {
        var userDeckRepo = _unitOfWork.GetRepository<UserDeck, Guid>();
        var userDeck = await userDeckRepo.Query(true)
            .Where(x => x.Id == userDeckId)
            .Include(deck => deck.Course)
            .Include(deck => deck.CustomLesson)
            .Select(ud => ud.ToUserDeckResponse())
            .FirstOrDefaultAsync(cancellationToken);

        if (userDeck == null)
        {
            _logger.LogWarning("User deck with Id: {id} not found!", userDeckId);
            return new ResponseData<UserDeckResponseDto>()
            {
                Succeeded = false,
                Message = "User deck not found."
            };
        }

        return new ResponseData<UserDeckResponseDto>()
        {
            Succeeded = true,
            Message = "User deck retrieved successfully.",
            Data = userDeck
        };
    }

    public async Task<ResponseData<Guid>> CreateUserDeckAsync(
        UserDeckDto request, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ResponseData<Guid>()
                {
                    Succeeded = false,
                    Message = "User not found!"
                };
            }

            var courseRepo = _unitOfWork.GetRepository<Course, Guid>();
            var courseId = request.CourseId ?? Guid.Empty;
            if (courseId != Guid.Empty)
            {
                var course = await courseRepo.GetByIdAsync(courseId, cancellationToken);
                if (course == null)
                {
                    return new ResponseData<Guid>()
                    {
                        Succeeded = false,
                        Message = "Course not found!"
                    };
                }
            }

            var customerLessonRepo = _unitOfWork.GetRepository<CustomLesson, Guid>();
            var customLessonId = request.CustomLessonId ?? Guid.Empty;
            if (customLessonId != Guid.Empty)
            {
                var customLesson = await customerLessonRepo.GetByIdAsync(customLessonId, cancellationToken);
                if (customLesson == null)
                {
                    return new ResponseData<Guid>()
                    {
                        Succeeded = false,
                        Message = "Custom lesson not found!"
                    };
                }
            }

            var userDeckRepo = _unitOfWork.GetRepository<UserDeck, Guid>();
            var userDeck = new UserDeck()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CourseId = courseId,
                CustomLessonId = customLessonId,
                CreatedAt = DateTimeOffset.Now
            };

            await userDeckRepo.AddAsync(userDeck, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResponseData<Guid>()
            {
                Succeeded = true,
                Message = "User deck created successfully!",
                Data = userDeck.Id
            };

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new ResponseData<Guid>()
            {
                Succeeded = false,
                Message = "Fail to get user deck!! Error: " + e.Message
            };
        }
    }


}