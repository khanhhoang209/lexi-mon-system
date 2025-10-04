using System.Security.Claims;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/lesson-progress")]
public class LessonProgressController : ControllerBase
{
    private readonly ILessonProgressService _service;

    public LessonProgressController(ILessonProgressService service)
    {
        _service = service;
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetLessonProgress(
        [FromQuery] GetLessonProgressRequest request, 
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new Exception("User not found");

        var result = await _service.GetLessonProgressAsync(userId, request, cancellationToken);
        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateLessonProgress(
        [FromBody] LessonProgressRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new Exception("User not found");

        var result = await _service.CreateLessonProgressAsync(userId, request, cancellationToken);
        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetLessonProgressById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetLessonProgressByIdAsync(id, cancellationToken);
        if (!result.Succeeded)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateLessonProgress(
        Guid id,
        [FromBody] LessonProgressRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateLessonProgressAsync(id, request, cancellationToken);
        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteLessonProgress(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteLessonProgressAsync(id, cancellationToken);
        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    

}