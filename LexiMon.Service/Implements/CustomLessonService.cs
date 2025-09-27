using LexiMon.Repository.Domains;
using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Mappers;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LexiMon.Service.Implements;

public class CustomLessonService : ICustomLessonService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    
    public CustomLessonService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ResponseData<Guid>> CreateCustomLessonAsync(
        CustomLessonRequestDto request, 
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ResponseData<Guid>
            {
                Succeeded = false,
                Message = "User not found",
                Data = Guid.Empty
            };
        }
        
        var repo = _unitOfWork.GetRepository<CustomLesson, Guid>();
        var newCustomLesson = request.ToLesson();

        var userDeckRepo = _unitOfWork.GetRepository<UserDeck, Guid>();
        var userDeck = new UserDeck
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CustomLessonId = newCustomLesson.Id,
            CreatedAt = DateTimeOffset.Now
        };
        
        await repo.AddAsync(newCustomLesson, cancellationToken);
        await userDeckRepo.AddAsync(userDeck, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ResponseData<Guid>
        {
            Succeeded = true,
            Message = "Custom lesson created successfully",
            Data = newCustomLesson.Id
        };
    }

    public async Task<ResponseData<Guid>> UpdateCustomLessonAsync(
        Guid id, 
        CustomLessonRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<CustomLesson, Guid>();
        var customLesson = await repo.GetByIdAsync(id, cancellationToken);
        
        if (customLesson == null)
        {
            return new ResponseData<Guid>
            {
                Succeeded = false,
                Message = $"Custom lesson with id {id}  not found",
                Data = Guid.Empty
            };
        }
        
        customLesson.UpdateCustomLesson(request);
        await repo.UpdateAsync(customLesson, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new ResponseData<Guid>
        {
            Succeeded = true,
            Message = "Custom lesson created successfully",
            Data = customLesson.Id
        };
    }

    public async Task<ResponseData<Guid>> DeleteCustomLessonAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<CustomLesson, Guid>();
        var customLesson = await repo.GetByIdAsync(id, cancellationToken);
        if (customLesson == null)
        {
            return new ResponseData<Guid>
            {
                Succeeded = false,
                Message = $"Custom lesson with id {id}  not found"
            };
        }
        var userDeckRepo = _unitOfWork.GetRepository<UserDeck, Guid>();
        var userDecks = await userDeckRepo.Query()
            .Where(ud => ud.CustomLessonId == customLesson.Id)
            .ToListAsync(cancellationToken);
       
        if (userDecks.Any())
        {
            foreach (var u in userDecks)
                await userDeckRepo.RemoveAsync(u, cancellationToken);
           
        }
        await repo.RemoveAsync(customLesson, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new ResponseData<Guid>
        {
            Succeeded = true,
            Message = "Custom lesson delete successfully",
            Data = customLesson.Id
        };
    }

    public async Task<ResponseData<CustomLessonResponseDto>> GetCustomLessonByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<CustomLesson, Guid>();
        var customLesson = await repo.Query()
            .Where(cl => cl.Id == id)
            .Select(cl => cl.ToCustomLessonResponse())
            .FirstOrDefaultAsync(cancellationToken);
        
        if (customLesson == null)
        {
            return new ResponseData<CustomLessonResponseDto>
            {
                Succeeded = false,
                Message = $"Custom lesson with id {id} not found"
            };
        }
        
        return new ResponseData<CustomLessonResponseDto>
        {
            Succeeded = true,
            Message = "Custom lesson retrieved successfully",
            Data = customLesson
        };
    }

    public async Task<ResponseData<List<CustomLessonResponseDto>>> GetCustomLessonsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<CustomLesson, Guid>();
        var customLessons = await repo.GetAllAsync(true, cancellationToken);
        var response = customLessons.Select(cl => cl.ToCustomLessonResponse()).ToList();
        
        return new ResponseData<List<CustomLessonResponseDto>>
        {
            Succeeded = true,
            Message = "Custom lessons retrieved successfully",
            Data = response
        };
    }
}