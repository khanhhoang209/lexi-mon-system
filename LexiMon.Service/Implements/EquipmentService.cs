using LexiMon.Repository.Domains;
using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LexiMon.Service.Implements;

public class EquipmentService : IEquipmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EquipmentService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public EquipmentService(
        IUnitOfWork unitOfWork, 
        ILogger<EquipmentService> logger, 
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _userManager = userManager;
    }
    
    public async Task<PaginatedResponse<EquipmentResponseDto>> GetEquipmentsAsync(
        string userId, 
        GetEquipmentRequest request, 
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User not found");
            return new PaginatedResponse<EquipmentResponseDto>()
            {
                Succeeded = false,
                Message = "User not found",
            };
        }
        
        
        var repo = _unitOfWork.GetRepository<Equipment, (Guid, Guid)>();
        var query = repo.Query()
            .Include(e => e.Character)
                .ThenInclude(c => c!.User)
            .Include(e => e.Item)
                .ThenInclude(i => i!.Category)
            .Where(e => e.Character!.UserId == userId);

        if (!string.IsNullOrEmpty(request.CategoryName))
            query = query.Where(e => e.Item!.Category!.Name.Contains(request.CategoryName));
        if (!string.IsNullOrEmpty(request.ItemName))
            query = query.Where(e => e.Item!.Name.Contains(request.ItemName));
        if(request.IsPremium.HasValue)
            query = query.Where(e => e.Item!.IsPremium == request.IsPremium);
        
        var totalCount = query.Count();
        var response = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EquipmentResponseDto
            {
                CharacterId = e.CharacterId,
                ItemId = e.ItemId,
                ItemName = e.Item!.Name,
                ImageUrl = e.Item.ImageUrl,
                CategoryId = e.Item.CategoryId,
                CategoryName = e.Item.Category!.Name,
                IsPremium = e.Item.IsPremium
            })
            .ToListAsync(cancellationToken);
        
        _logger.LogInformation("Item retrieved successfully");
        return new PaginatedResponse<EquipmentResponseDto>()
        {
            Succeeded = true,
            Message = "Item retrieved successfully",
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            Data = response
        };
    }
}