using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface ICategoryService
{
    Task<ResponseData<Guid>> CreateCategoryAsync(
        CategoryRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse> UpdateCategoryAsync(
        Guid id,
        CategoryRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse> DeleteCategoryAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedResponse<CategoryResponseDto>> GetCategoryAsync(
        GetCategoryRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<CategoryResponseDto>> GetCategoryByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

}