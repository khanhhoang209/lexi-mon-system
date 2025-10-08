using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/achievements")]
public class AchievementsController : ControllerBase
{
    private readonly IAchievementService _service;
    public AchievementsController(IAchievementService service)
    {
        _service = service;
    }
    
     [HttpPost]
    public async Task<IResult> CreateAsync(
        [FromBody]  AchievementRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.CreateAchievementAsync(request, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Created($"/api/animation-types/{serviceResponse.Data}", serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] AchievementRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.UpdateAchievementAsync(id, request, cancellationToken);
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
        var serviceResponse = await _service.DeleteAchievementAsync(id, cancellationToken);
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
        var serviceResponse = await _service.GetAchievementByIdAsync(id, cancellationToken);
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
        var serviceResponse = await _service.GetAchievementsAsync(request, cancellationToken);
        return Results.Ok(serviceResponse);
        
    }
}