using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }
    [HttpPost]
    public async Task<IResult> CreateAsync(
        [FromBody] CategoryRequestDto productRequest,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.CreateCategoryAsync(productRequest, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Created($"/api/products/{serviceResponse.Data}", serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }

    [HttpPut("{id:guid}")]
    public async Task<IResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] CategoryRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.UpdateCategoryAsync(id, request, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.DeleteCategoryAsync(id);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound(serviceResponse);
    }
    
    [HttpGet]
    public async Task<IResult> GetAllAsync(
        [FromQuery] GetCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetCategoryAsync( request, cancellationToken);
        if (serviceResponse.Succeeded)
            return TypedResults.Ok(serviceResponse);
    
        return TypedResults.BadRequest(serviceResponse);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IResult> GetByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetCategoryByIdAsync(id, cancellationToken);
        if (serviceResponse.Succeeded)
            return TypedResults.Ok(serviceResponse);
    
        return TypedResults.NotFound(serviceResponse);
    }
    
}