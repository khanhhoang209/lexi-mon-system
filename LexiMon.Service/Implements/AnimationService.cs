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

public class AnimationService : IAnimationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AnimationService> _logger;

    public AnimationService(IUnitOfWork unitOfWork, ILogger<AnimationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResponseData<Guid>> CreateAnimationAsync(
        AnimationRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var itemRepo = _unitOfWork.GetRepository<Item, Guid>();
            var item = itemRepo.Query().FirstOrDefault(i => i.Id == request.ItemId);
            if (item == null)
            {
                _logger.LogError($"Item with id {request.ItemId} not found");
                return new ResponseData<Guid>()
                {
                    Succeeded = false,
                    Message = $"Item with id {request.ItemId} not found"
                };
            }
            
            var animationTypeRepo = _unitOfWork.GetRepository<AnimationType, Guid>();
            var animationType = animationTypeRepo.Query().FirstOrDefault(at => at.Id == request.AnimationTypeId);
            if (animationType == null)
            {
                _logger.LogError($"Animation Type with id {request.AnimationTypeId} not found");
                return new ResponseData<Guid>()
                {
                    Succeeded = false,
                    Message = $"Animation Type with id {request.AnimationTypeId} not found"
                };
            }
            
            var repo = _unitOfWork.GetRepository<Animation, Guid>();
            var animation = request.ToAnimation();
            await repo.AddAsync(animation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Create Animation Success!");
            return new ResponseData<Guid>()
            {
                Succeeded = true,
                Message = "Create Animation Success!",
                Data = animation.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Create Animation Failed!");
            return new ResponseData<Guid>()
            {
                Succeeded = false,
                Message = "Create Animation Failed!" + ex.Message,
            };
        }
    }

    public async Task<ServiceResponse> UpdateAnimationAsync(
        Guid id, 
        AnimationRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Animation, Guid>();
        var animation = repo.Query().FirstOrDefault(a => a.Id == id);
        if (animation == null)
        {
            _logger.LogWarning("Animation Not Found!");
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Animation Not Found!"
            };
        }
        
        animation.UpdateAnimation(request);
        await repo.UpdateAsync(animation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Update Animation Success!");
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = "Update Animation Success!",
        };
    }

    public async Task<ServiceResponse> DeleteAnimationAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Animation, Guid>();
        var animation = repo.Query().FirstOrDefault(a => a.Id == id);
        if (animation == null)
        {
            _logger.LogWarning("Animation Not Found!");
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Animation Not Found!"
            };
        }
        
        await repo.RemoveAsync(animation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Delete Animation Success!");
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = "Delete Animation Success!"
        };
    }

    public async Task<ResponseData<AnimationResponseDto>> GetAnimationByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Animation, Guid>();
        var animation = await repo.Query()
            .Include(a => a.AnimationType)
            .Include(a => a.Item)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (animation == null)
        {
            _logger.LogWarning("Animation Not Found!");
            return new ResponseData<AnimationResponseDto>()
            {
                Succeeded = false,
                Message = "Animation Not Found!"
            };
        }

        var response = animation.ToAnimationResponse();
        
        _logger.LogInformation("Animation with id: {id} retrieved successfully", id);
        return new ResponseData<AnimationResponseDto>()
        {
            Succeeded = true,
            Message = "Animation retrieved success!",
            Data = response
        };
    }

    public async Task<PaginatedResponse<AnimationResponseDto>> GetAnimationsAsync(
        GetAnimationRequest request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Animation, Guid>();
        var query = repo.Query();
            
        query = query.Include(a => a.AnimationType)
            .Include(a => a.Item);
        
        if(!string.IsNullOrEmpty(request.AnimationName))
            query = query.Where(a => a.Name!.Contains(request.AnimationName));
        
        if(!string.IsNullOrEmpty(request.AnimationTypeName))
            query = query.Where(a => a.AnimationType!.Name.Contains(request.AnimationName));

        if(!string.IsNullOrEmpty(request.ItemName))
            query = query.Where(a => a.Item!.Name.Contains(request.ItemName));

        if(request.IsActive.HasValue)
            query = query.Where(a => a.Status == request.IsActive.Value);
        
        var totalCount = query.Count();
        var response = await query
            .OrderByDescending(a => a.Status)
            .ThenByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => a.ToAnimationResponse())
            .ToListAsync(cancellationToken);
        return new PaginatedResponse<AnimationResponseDto>()
        {
            Succeeded = true,
            Message = "Animation retrieved successfully.",
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            Data = response
        };
    }
}