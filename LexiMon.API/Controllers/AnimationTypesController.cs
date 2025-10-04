using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/animation-types")]
public class AnimationTypesController : ControllerBase
{
    private readonly IAnimationTypeService _service;

    public AnimationTypesController(IAnimationTypeService service)
    {
        _service = service;
    }
    
    [HttpPost]
    public async Task<IResult> CreateAsync(
        [FromBody]  AnimationTypeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.CreateAnimationTypeAsync(request, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Created($"/api/animation-types/{serviceResponse.Data}", serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] AnimationTypeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.UpdateAnimationTypeAsync(id, request, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeleteAsync(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.DeleteAnimationTypeAsync(id, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound(serviceResponse);
    }

    [HttpGet("{id:guid}")]
    public async Task<IResult> GetByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetAnimationTypeById(id, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return TypedResults.BadRequest();
        }
        return TypedResults.Ok(serviceResponse);
    }

    [HttpGet]
    public async Task<IResult> GetAllAsync(
        [FromQuery] GetAnimationTypeRequest request, 
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetAllAnimationTypesAsync(request, cancellationToken);
        return Results.Ok(serviceResponse);
        
    }
}