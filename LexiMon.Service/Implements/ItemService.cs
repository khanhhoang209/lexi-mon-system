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

public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ItemService> _logger;

    public ItemService(IUnitOfWork unitOfWork, ILogger<ItemService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }


    public async Task<ResponseData<Guid>> CreateItemAsync(
        ItemRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var categoryRepo = _unitOfWork.GetRepository<Category, Guid>();
            var category = categoryRepo.Query().FirstOrDefault(x => x.Id == request.CategoryId);
            if (category == null)
            {
                _logger.LogWarning($"Category {request.CategoryId} not found");
                return new ResponseData<Guid>()
                {
                    Succeeded = false,
                    Message = $"Category {request.CategoryId} not found"
                };
            }
            
            var repo = _unitOfWork.GetRepository<Item, Guid>();
            var item = request.ToItem();
            
            await repo.AddAsync(item, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Item {item.Id} created");
            return new ResponseData<Guid>()
            {
                Succeeded = true,
                Message = $"Item {item.Id} created success!",
                Data = item.Id
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new ResponseData<Guid>()
            {
                Succeeded = false,
                Message = $"Create Item fail! Error: {e.Message}"
            };
        }
    }

    public async Task<ServiceResponse> UpdateItemAsync(
        Guid id, 
        ItemRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var categoryRepo = _unitOfWork.GetRepository<Category, Guid>();
            var category = categoryRepo.Query().FirstOrDefault(x => x.Id == request.CategoryId);
            if (category == null)
            {
                _logger.LogWarning($"Category {request.CategoryId} not found");
                return new ServiceResponse()
                {
                    Succeeded = false,
                    Message = $"Category {request.CategoryId} not found"
                };
            }
            
            var repo = _unitOfWork.GetRepository<Item, Guid>();
            var item = repo.Query().FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                _logger.LogWarning($"Item {id} not found");
                return new ServiceResponse()
                {
                    Succeeded = false,
                    Message = $"Item {id} not found"
                };
            }
            
            item.UpdateItem(request);
            await repo.UpdateAsync(item, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Item {item.Id} updated");
            return new ServiceResponse()
            {
                Succeeded = true,
                Message = $"Item {item.Id} updated success!"
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = $"Update Item fail! Error: {e.Message}"
            };
        }
    }

    public async Task<ServiceResponse> DeleteItemAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Item, Guid>();
        var item = repo.Query().FirstOrDefault(x => x.Id == id);

        if (item == null)
        {
            _logger.LogWarning($"Item {id} not found");
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = $"Item {id} not found"
            };
        }
        
        await repo.RemoveAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Item {id} deleted");
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = $"Item {id} removed"
        };
    }

    public async Task<ResponseData<ItemResponseDto>> GetItemByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Item, Guid>();
        var item = await repo.Query()
            .Include(i => i.Category)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (item == null)
        {
            _logger.LogWarning($"Item {id} not found");
            return new ResponseData<ItemResponseDto>()
            {
                Succeeded = false,
                Message = $"Item {id} not found"
            };
        }
        _logger.LogInformation("Item with id: {id} retrieved successfully", id);
        var response = item.ToItemResponse();
        return new ResponseData<ItemResponseDto>()
        {
            Succeeded = true,
            Message = "Item retrieved successfully",
            Data = response
        };

    }

    public async Task<PaginatedResponse<ItemResponseDto>> GetItemsAsync(
        GetItemRequest request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Item, Guid>();
        var query = repo.Query();
        query = query.Include(i => i.Category);
        
        if(!string.IsNullOrEmpty(request.ItemName))
            query = query.Where(i => i.Name.Contains(request.ItemName));
        
        if(!string.IsNullOrEmpty(request.CategoryName))
            query = query.Where(i => i.Category!.Name.Contains(request.CategoryName));
        
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
        if(request.IsActive.HasValue)
            query = query.Where(a => a.Status == request.IsActive.Value);
        if(request.IsPremium.HasValue)
            query = query.Where(p => p.IsPremium == request.IsPremium.Value);
        
        var totalCount = query.Count();
        
        var itemsResponse = await query
            .OrderByDescending(c => c.Status)
            .ThenByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => c.ToItemResponse())
            .ToListAsync(cancellationToken);
        
        _logger.LogInformation("Item retrieved successfully");
        return new PaginatedResponse<ItemResponseDto>()
        {
            Succeeded = true,
            Message = "Item retrieved successfully",
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            Data = itemsResponse
        };
    }
    public async Task<PaginatedResponse<ItemResponseDto>> GetShopItemsAsync(
    string userId,
    GetItemRequest request,
    CancellationToken cancellationToken = default) 
    {
        var character = await _unitOfWork.GetRepository<Character, Guid>()
            .Query()
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (character == null)
        {
            _logger.LogWarning("Character not found for user {UserId}", userId);
            return new PaginatedResponse<ItemResponseDto>
            {
                Succeeded = false,
                Message = "Character not found"
            };
        }

        var itemRepo = _unitOfWork.GetRepository<Item, Guid>()
                                                    .Query().Include(i => i.Category).Where(c => c.Status);;
        var equipRepo = _unitOfWork.GetRepository<Equipment, (Guid, Guid)>().Query();

        // chỉ lấy item mà user chưa sở hữu (NOT EXISTS)
        var query = itemRepo
            .Where(i => !equipRepo.Any(e => e.ItemId == i.Id && e.CharacterId == character.Id));

        // Áp các filter hiện có
        if (!string.IsNullOrEmpty(request.ItemName))
            query = query.Where(i => i.Name.Contains(request.ItemName));

        if (!string.IsNullOrEmpty(request.CategoryName))
            query = query.Where(i => i.Category!.Name.Contains(request.CategoryName));

        if (request.MinPrice != null && request.MaxPrice != null)
            query = query.Where(i => (i.Price >= request.MinPrice && i.Price <= request.MaxPrice)
                                  || (i.Coin  >= request.MinPrice && i.Coin  <= request.MaxPrice));
        else if (request.MinPrice != null)
            query = query.Where(i => i.Price >= request.MinPrice || i.Coin >= request.MinPrice);
        else if (request.MaxPrice != null)
            query = query.Where(i => i.Price <= request.MaxPrice || i.Coin <= request.MaxPrice);

        
        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => i.ToItemResponse())
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<ItemResponseDto>
        {
            Succeeded = true,
            Message = "Shop items retrieved successfully",
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            Data = data
        };
    }

}