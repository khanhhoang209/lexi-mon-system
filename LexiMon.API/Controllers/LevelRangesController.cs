using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/level-ranges")]
public class LevelRangesController : ControllerBase
{
  private readonly ILevelRangeService _service;
    public LevelRangesController(ILevelRangeService service)
    {
        _service = service;
    }
     [HttpPost]
    public async Task<IResult> CreateAsync(
        [FromBody]  LevelRangeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.CreateLevelRangeAsync(request, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Created($"/api/animation-types/{serviceResponse.Data}", serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] LevelRangeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.UpdateLevelRangeAsync(id, request, cancellationToken);
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
        var serviceResponse = await _service.DeleteLevelRangeAsync(id, cancellationToken);
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
        var serviceResponse = await _service.GetLevelRangeByIdAsync(id, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return TypedResults.BadRequest();
        }
        return TypedResults.Ok(serviceResponse);
    }

    [HttpGet]
    public async Task<IResult> GetAllAsync(
        [FromQuery] GetBaseRequest request, 
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetLevelRangesAsync(request, cancellationToken);
        return Results.Ok(serviceResponse);
        
    }
}