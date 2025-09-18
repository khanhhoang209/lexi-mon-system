using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;

namespace LexiMon.Service.Interfaces;

public interface IProductService
{
    Task<ResponseData<Guid>> CreateProductAsync(
        ProductDto request,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse> UpdateProductAsync(
        Guid id,
        ProductDto request,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse> DeleteProductAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}