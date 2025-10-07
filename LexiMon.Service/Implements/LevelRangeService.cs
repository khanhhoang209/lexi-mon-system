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

public class LevelRangeService : ILevelRangeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LevelRangeService> _logger;

    public LevelRangeService(IUnitOfWork unitOfWork, ILogger<LevelRangeService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResponseData<Guid>> CreateLevelRangeAsync(
        LevelRangeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<LevelRange, Guid>();
        var levelRange = request.ToLevelRange();
        await repo.AddAsync(levelRange, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"LevelRange {levelRange.Id} created");
        return new ResponseData<Guid>()
        {
            Succeeded = true,
            Message = "LevelRange created success!",
            Data = levelRange.Id
        };
    }

    public async Task<ServiceResponse> UpdateLevelRangeAsync(
        Guid id,
        LevelRangeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<LevelRange, Guid>();
        var levelRange = await repo.GetByIdAsync(id, cancellationToken);
        if (levelRange == null)
        {
            _logger.LogWarning($"LevelRange {id} not found");
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = $"LevelRange {id} not found"
            };
        }

        levelRange.UpdateLevelRange(request);
        await repo.UpdateAsync(levelRange, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"LevelRange {id} updated");
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = $"LevelRange {id} updated success!"
        };
    }

    public async Task<ServiceResponse> DeleteLevelRangeAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<LevelRange, Guid>();
        var levelRange = await repo.GetByIdAsync(id, cancellationToken);
        if (levelRange == null)
        {
            _logger.LogWarning($"LevelRange {id} not found");
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = $"LevelRange {id} not found"
            };
        }

        await repo.RemoveAsync(levelRange, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"LevelRange {id} deleted");
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = $"LevelRange {id} deleted success!"
        };
    }

    public async Task<ResponseData<LevelRangeResponseDto>> GetLevelRangeByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<LevelRange, Guid>();
        var levelRange = await repo.GetByIdAsync(id, cancellationToken);
        if (levelRange == null)
        {
            _logger.LogWarning($"LevelRange {id} not found");
            return new ResponseData<LevelRangeResponseDto>()
            {
                Succeeded = false,
                Message = $"LevelRange {id} not found"
            };
        }

        _logger.LogInformation($"LevelRange {id} retrieved");
        return new ResponseData<LevelRangeResponseDto>()
        {
            Succeeded = true,
            Message = $"LevelRange {id} retrieved success!",
            Data = levelRange.ToResponse()
        };
    }

    public async Task<PaginatedResponse<LevelRangeResponseDto>> GetLevelRangesAsync(
        GetBaseRequest request,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<LevelRange, Guid>();
        var query = repo.Query();

        if (!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(lr => lr.Name.Contains(request.Name));
        }

        var totalCount = query.Count();

        var levelRange = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => l.ToResponse())
            .ToListAsync(cancellationToken);
        _logger.LogInformation($"LevelRanges retrieved: {levelRange.Count} items");
        return new PaginatedResponse<LevelRangeResponseDto>()
        {
            Succeeded = true,
            Message = "LevelRanges retrieved success!",
            Data = levelRange,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
        };
    }
}