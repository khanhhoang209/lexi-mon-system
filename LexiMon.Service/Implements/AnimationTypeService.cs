using LexiMon.Repository.Domains;
using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LexiMon.Service.Implements;

public class AnimationTypeService : IAnimationTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AnimationTypeService> _logger;

    public AnimationTypeService(IUnitOfWork unitOfWork, ILogger<AnimationTypeService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }


    public async Task<ResponseData<Guid>> CreateAnimationTypeAsync(
        AnimationTypeRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<AnimationType, Guid>();
        var id = Guid.NewGuid();
        var animationType = new AnimationType
        {
            Id = id,
            Name = request.Name
        };
        await repo.AddAsync(animationType, cancellationToken);
        await repo.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Create Animation Type Success!");
        return new ResponseData<Guid>()
        {
            Succeeded = true,
            Message = "Create Animation Type Success!",
            Data = id
        };
    }

    public async Task<ServiceResponse> UpdateAnimationTypeAsync(
        Guid id,
        AnimationTypeRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<AnimationType, Guid>();
        var animationType = await repo.Query().FirstOrDefaultAsync(at => at.Id == id, cancellationToken);
        if (animationType == null)
        {
            _logger.LogWarning("Not Found Animation Type With Id: {id}", id);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Not Found!"
            };
        }
        animationType.Name = request.Name;
        
        await repo.UpdateAsync(animationType, cancellationToken);
        await repo.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Update Animation Type Success!");
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = "Update Animation Type Success!",
        };
    }

    public async Task<ServiceResponse> DeleteAnimationTypeAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<AnimationType, Guid>();
        var animationType = await repo.Query().FirstOrDefaultAsync(at => at.Id == id, cancellationToken);
        if (animationType == null)
        {
            _logger.LogWarning("Not Found Animation Type With Id: {id}", id);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Not Found!"
            };
        }
        await repo.RemoveAsync(animationType, cancellationToken);
        await repo.SaveChangesAsync(cancellationToken);
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = "Delete Animation Type Success!",
        };
    }

    public async Task<ResponseData<AnimationTypeResponseDto>> GetAnimationTypeById(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<AnimationType, Guid>();
        var animationType = await repo.Query().FirstOrDefaultAsync(at => at.Id == id, cancellationToken);
        if (animationType == null)
        {
            _logger.LogWarning("Not Found Animation Type With Id: {id}", id);
            return new ResponseData<AnimationTypeResponseDto>()
            {
                Succeeded = false,
                Message = "Not Found!"
            };
        }

        var response = new AnimationTypeResponseDto()
        {
            Id = animationType.Id,
            Name = animationType.Name,
            IsActive = animationType.Status,
            CreatedAt = animationType.CreatedAt
        };

        return new ResponseData<AnimationTypeResponseDto>()
        {
            Succeeded = true,
            Message = "Animation Type retrieved successfully!",
            Data = response
        };
        
    }


    public async Task<PaginatedResponse<AnimationTypeResponseDto>> GetAllAnimationTypesAsync(
        GetAnimationTypeRequest request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<AnimationType, Guid>();

        var query = repo.Query().AsNoTracking();

        if (!string.IsNullOrEmpty(request.AnimationTypeName))
        {
            query = query.Where(at => at.Name.Contains(request.AnimationTypeName));
        }

        var totalCount = query.Count();
        var respone = await query
            .OrderByDescending(at => at.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(at => new AnimationTypeResponseDto()
            {
                Id = at.Id,
                Name = at.Name,
                IsActive = at.Status,
                CreatedAt = at.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<AnimationTypeResponseDto>()
        {
            Succeeded = true,
            Message = "Courses retrieved successfully",
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            Data = respone
        };
    }

}