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

public class EnemyLevelService : IEnemyLevelService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EnemyLevelService> _logger;

    public EnemyLevelService(IUnitOfWork unitOfWork, ILogger<EnemyLevelService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResponseData<Guid>> CreateEnemyLevelAsync(
        EnemyLevelRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<EnemyLevel, Guid>();
        var enemyLevel = request.ToEnemyLevel();
        
        await repo.AddAsync(enemyLevel, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Enemy Level {enemyLevel.Id} created");
        return new ResponseData<Guid>()
        {
            Succeeded = true,
            Message = "Enemy Level created success!",
            Data = enemyLevel.Id
        };
    }

    public async Task<ServiceResponse> UpdateEnemyLevelAsync(
        Guid id, 
        EnemyLevelRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<EnemyLevel, Guid>();
        var enemyLevel = await repo.GetByIdAsync(id, cancellationToken);
        if (enemyLevel == null)
        {
            _logger.LogWarning($"Enemy Level {id} not found");
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = $"Enemy Level {id} not found"
            };
        }
        
        enemyLevel.UpdateEnemyLevel(request); 
        await repo.UpdateAsync(enemyLevel, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Enemy Level {id} updated");
        return new ServiceResponse() 
        { 
            Succeeded = true, 
            Message = $"Enemy Level {id} updated success!"
        };
        
    }

    public async Task<ServiceResponse> DeleteEnemyLevelAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<EnemyLevel, Guid>();
        var enemyLevel = await repo.GetByIdAsync(id, cancellationToken);
        if (enemyLevel == null)
        {
            _logger.LogWarning($"Enemy Level {id} not found");
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = $"Enemy Level {id} not found"
            };
        }
        
        await repo.RemoveAsync(enemyLevel, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Enemy Level {id} deleted");
        return new ServiceResponse() 
        { 
            Succeeded = true, 
            Message = $"Enemy Level {id} deleted success!"
        };
    }

    public async Task<ResponseData<EnemyLevelResponseDto>> GetEnemyLevelByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<EnemyLevel, Guid>();
        var enemyLevel = await repo.GetByIdAsync(id, cancellationToken);
        if (enemyLevel == null)
        {
            _logger.LogWarning($"Enemy Level {id} not found");
            return new ResponseData<EnemyLevelResponseDto>()
            {
                Succeeded = false,
                Message = $"Enemy Level {id} not found",
                Data = null
            };
        }
        
        _logger.LogInformation($"Enemy Level {id} found");
        return new ResponseData<EnemyLevelResponseDto>()
        {
            Succeeded = true,
            Message = $"Enemy Level {id} found",
            Data = enemyLevel.ToEnemyLevelResponse()
        };
    }

    public async Task<PaginatedResponse<EnemyLevelResponseDto>> GetEnemyLevelsAsync(
        GetBaseRequest request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<EnemyLevel, Guid>();
        var query = repo.Query();

        if (!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(x => x.Name.Contains(request.Name));
        }

        var totalCount = query.Count();
        
        var enemyLevels = await query
            .OrderByDescending(el => el.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        
        var response = enemyLevels.Select(x => x.ToEnemyLevelResponse()).ToList();
        
        _logger.LogInformation("Enemy Levels retrieved successfully");
        return new PaginatedResponse<EnemyLevelResponseDto>()
        {
            Succeeded = true,
            Message = "Enemy Levels retrieved successfully",
            Data = response,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
        };
    }
}