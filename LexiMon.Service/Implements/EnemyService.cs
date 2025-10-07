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

public class EnemyService : IEnemyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EnemyService> _logger;

    public EnemyService(IUnitOfWork unitOfWork, ILogger<EnemyService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResponseData<Guid>> CreateEnemyAsync(
        EnemyRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Enemy, Guid>();

        var enemy = request.ToEnemy();
        await repo.AddAsync(enemy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Enemy {enemy.Id} created");
        return new ResponseData<Guid>()
        {
            Succeeded = true,
            Message = "Enemy created success!",
            Data = enemy.Id
        };
    }

    public async Task<ServiceResponse> UpdateEnemyAsync(
        Guid id, 
        EnemyRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Enemy, Guid>();
        var enemy = await repo.GetByIdAsync(id, cancellationToken);
        if (enemy == null)
        {
            _logger.LogWarning($"Enemy {id} not found");
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = $"Enemy {id} not found"
            };
        }
        enemy.UpdateEnemy(request); 
        await repo.UpdateAsync(enemy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Enemy {id} updated");
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = $"Enemy {id} updated success!"
        };
    }

    public async Task<ServiceResponse> DeleteEnemyAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Enemy, Guid>();
        var enemy = await repo.GetByIdAsync(id, cancellationToken);
        if (enemy == null)
        {
            _logger.LogWarning($"Enemy {id} not found");
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = $"Enemy {id} not found"
            };
        }
        enemy.Status = false;
        await repo.RemoveAsync(enemy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Enemy {id} deleted");
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = $"Enemy {id} deleted success!"
        };
    }

    public async Task<ResponseData<EnemyResponseDto>> GetEnemyByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Enemy, Guid>();
        var enemy = await repo.Query()
            .Include(e => e.EnemyLevel)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (enemy == null)
        {
            _logger.LogWarning($"Enemy {id} not found");
            return new ResponseData<EnemyResponseDto>()
            {
                Succeeded = false,
                Message = $"Enemy {id} not found"
            };
        }
        return new ResponseData<EnemyResponseDto>()
        {
            Succeeded = true,
            Message = "Get Enemy success!",
            Data = enemy.ToEnemyResponse()
        };
    }

    public async Task<PaginatedResponse<EnemyResponseDto>> GetEnemiesAsync(
        GetBaseRequest request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Enemy, Guid>();
        var query = repo.Query();
        query = query.Include(e => e.EnemyLevel);

        if (!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(e => e.Name.Contains(request.Name));
        }

        var totalCount = query.Count();
        var enemies = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => e.ToEnemyResponse())
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<EnemyResponseDto>()
        {
            Succeeded = true,
            Message = "Get Enemies success!",
            Data = enemies,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
        };
    }
}