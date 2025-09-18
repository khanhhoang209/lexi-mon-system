using LexiMon.Repository.Domains;
using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;

namespace LexiMon.Service.Implements;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseData<Guid>> CreateProductAsync(ProductDto request, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Product, Guid>();
        var id = Guid.NewGuid();
        var product = new Product()
        {
            Id = id,
            Name = request.Name
        };

        await repo.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ResponseData<Guid>()
        {
            Succeeded = true,
            Message = "Product created successfully!",
            Data = id
        };
    }

    public async Task<ServiceResponse> UpdateProductAsync(Guid id, ProductDto request, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Product, Guid>();

        var product = await repo.GetByIdAsync(id, cancellationToken);

        if (product == null!)
        {
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Product not found!"
            };
        }

        product.Name = request.Name;
        await repo.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ResponseData<Guid>()
        {
            Succeeded = true,
            Message = "Product updated successfully!",
            Data = id
        };
    }

    public async Task<ServiceResponse> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Product, Guid>();

        var product = await repo.GetByIdAsync(id, cancellationToken);

        if (product == null!)
        {
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Product not found!"
            };
        }

        await repo.RemoveAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ServiceResponse()
        {
            Succeeded = true,
            Message = "Product deleted successfully!"
        };
    }
}