using LexiMon.Repository.Domains;
using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LexiMon.Service.Implements;

public class CategoryService : ICategoryService
{
    private IUnitOfWork _unitOfWork;
    private ILogger<CategoryService> _logger;

    public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResponseData<Guid>> CreateCategoryAsync(
        CategoryRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Category, Guid>();
        var id = Guid.NewGuid();
        var category = new Category()
        {
            Id = id,
            Name = request.Name
        };

        await repo.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Category created successfully");
        return new ResponseData<Guid>()
        {
            Succeeded = true,
            Message = "Category created successfully!",
            Data = id
        };
    }

    public async Task<ServiceResponse> UpdateCategoryAsync(Guid id, 
        CategoryRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Category, Guid>();

        var category = await repo.GetByIdAsync(id, cancellationToken);

        if (category == null!)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Category not found!"
            };
        }

        category.Name = request.Name;
        await repo.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Category updated successfully");
        return new ResponseData<Guid>()
        {
            Succeeded = true,
            Message = "Category updated successfully!",
            Data = id
        };
    }

    public async Task<ServiceResponse> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Category, Guid>();

        var category = await repo.GetByIdAsync(id, cancellationToken);

        if (category == null!)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Category not found!"
            };
        }

        await repo.RemoveAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ServiceResponse()
        {
            Succeeded = true,
            Message = "Category deleted successfully!"
        };
    }

    public async Task<PaginatedResponse<CategoryResponseDto>> GetCategoryAsync(
        GetCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Category, Guid>();
        var query =repo.Query().AsNoTracking();
        
        if (!string.IsNullOrEmpty(request.Name))
            query = query.Where(c => c.Name!.Contains(request.Name));
        if(request.IsActive.HasValue)
            query = query.Where(a => a.Status == request.IsActive.Value);
        var total = query.Count();
        var response = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CategoryResponseDto 
                { 
                    Id = c.Id, 
                    Name = c.Name,
                    IsActive = c.Status,
                    CreateAt = c.CreatedAt
                })
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<CategoryResponseDto>
        {
            Succeeded = true,
            Message = "Categories retrieved successfully!",
            Data = response,
            TotalCount = total,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)total / request.PageSize),
        };
    }

    public async Task<ResponseData<CategoryResponseDto>> GetCategoryByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Category, Guid>();
        var category = await repo.GetByIdAsync(id, cancellationToken);

        if (category == null)
        {
            return new ResponseData<CategoryResponseDto>
            {
                Succeeded = false,
                Message = "Category not found!"
            };
        }

        var result = new CategoryResponseDto
        {
            Id = category.Id,
            Name = category!.Name,
            IsActive = category.Status,
            CreateAt = category.CreatedAt
        };

        return new ResponseData<CategoryResponseDto>
        {
            Succeeded = true,
            Message = "Category retrieved successfully!",
            Data = result
        };
    }

}