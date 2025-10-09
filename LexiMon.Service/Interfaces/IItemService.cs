using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface IItemService
{
    Task<ResponseData<Guid>> CreateItemAsync(
        ItemRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> UpdateItemAsync(
        Guid id,
        ItemRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> DeleteItemAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<ItemResponseDto>> GetItemByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedResponse<ItemResponseDto>> GetItemsAsync(
        GetItemRequest request,
        CancellationToken cancellationToken = default);

    Task<PaginatedResponse<ItemResponseDto>> GetShopItemsAsync(
        string userId,
        GetItemRequest request,
        CancellationToken cancellationToken = default);
}