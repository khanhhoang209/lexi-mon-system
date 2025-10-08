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

public class AchievementService : IAchievementService
{
    private readonly ILogger<AchievementService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    
    public AchievementService(ILogger<AchievementService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ResponseData<Guid>> CreateAchievementAsync(
        AchievementRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var repo = _unitOfWork.GetRepository<Achievement, Guid>();
            var achievement = request.ToAchievement();
            await repo.AddAsync(achievement, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Create achievement successfully: {AchievementId}", achievement.Id);
            return new ResponseData<Guid>
            {
                Succeeded = true,
                Message = "Create achievement successfully",
                Data = achievement.Id
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when creating achievement");
            return new ResponseData<Guid>
            {
                Succeeded = true,
                Message = "Error when creating achievement: " + e.Message,
            };
        }
    }

    public async Task<ServiceResponse> UpdateAchievementAsync(
        Guid id, 
        AchievementRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var repo = _unitOfWork.GetRepository<Achievement, Guid>();
            var achievement = await repo.GetByIdAsync(id, cancellationToken);
            if (achievement == null)
            {
                _logger.LogWarning("Achievement {AchievementId} not found", id);
                return new ServiceResponse()
                {
                    Succeeded = false,
                    Message = $"Achievement {id} not found"
                };
            }
            
            achievement.UpdateAchievement(request);
            await repo.UpdateAsync(achievement, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Update achievement successfully: {AchievementId}", achievement.Id);
            return new ServiceResponse()
            {
                Succeeded = true,
                Message = "Update achievement successfully"
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when updating achievement {AchievementId}", id);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Error when updating achievement: " + e.Message
            };
        }
    }

    public async Task<ServiceResponse> DeleteAchievementAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var repo = _unitOfWork.GetRepository<Achievement, Guid>();
            var achievement = await repo.GetByIdAsync(id, cancellationToken);
            if (achievement == null)
            {
                _logger.LogWarning("Achievement {AchievementId} not found", id);
                return new ServiceResponse()
                {
                    Succeeded = false,
                    Message = $"Achievement {id} not found"
                };
            }
            
            await repo.RemoveAsync(achievement, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Delete achievement successfully: {AchievementId}", achievement.Id);
            return new ServiceResponse()
            {
                Succeeded = true,
                Message = "Delete achievement successfully"
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when deleting achievement {AchievementId}", id);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Error when deleting achievement: " + e.Message
            };
        }
    }

    public async Task<ResponseData<AchievementResponseDto>> GetAchievementByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var repo = _unitOfWork.GetRepository<Achievement, Guid>();
            var achievement = await repo.GetByIdAsync(id, cancellationToken);
            if (achievement == null)
            {
                _logger.LogWarning("Achievement {AchievementId} not found", id);
                return new ResponseData<AchievementResponseDto>()
                {
                    Succeeded = false,
                    Message = $"Achievement {id} not found"
                };
            }
            
            _logger.LogInformation("Get achievement successfully: {AchievementId}", achievement.Id);
            return new ResponseData<AchievementResponseDto>()
            {
                Succeeded = true,
                Message = "Get achievement successfully",
                Data = achievement.ToResponse()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when getting achievement {AchievementId}", id);
            return new ResponseData<AchievementResponseDto>()
            {
                Succeeded = false,
                Message = "Error when getting achievement: " + e.Message
            };
        }
    }

    public async Task<PaginatedResponse<AchievementResponseDto>> GetAchievementsAsync(
        GetBaseRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var repo = _unitOfWork.GetRepository<Achievement, Guid>();
            var query = repo.Query();
            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(a => a.Name.Contains(request.Name));
            }
            if (request.IsActive.HasValue)
            {
                query = query.Where(a => a.Status == request.IsActive.Value);
            }
            var totalCount = query.Count();
            var response = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(a => a.ToResponse())
                .ToListAsync(cancellationToken);
            
           _logger.LogInformation("Get achievements successfully");
            return new PaginatedResponse<AchievementResponseDto>()
            {
                Succeeded = true,
                Message = "Get achievements successfully",
                Data = response,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when getting achievements");
            return new PaginatedResponse<AchievementResponseDto>()
            {
                Succeeded = false,
                Message = "Error when getting achievements: " + e.Message
            };
        }
    }
}