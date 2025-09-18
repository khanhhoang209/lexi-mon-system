using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IResult> CreateAsync(
        [FromBody] ProductDto productRequest,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _productService.CreateProductAsync(productRequest, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Created($"/api/products/{serviceResponse.Data}", serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }

    [HttpPut("{id:guid}")]
    public async Task<IResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] ProductDto productRequest,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _productService.UpdateProductAsync(id, productRequest, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _productService.DeleteProductAsync(id);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound(serviceResponse);
    }

}