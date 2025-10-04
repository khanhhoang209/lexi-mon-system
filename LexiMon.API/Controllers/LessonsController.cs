using System.Security.Claims;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/lessons")]
public class LessonsController : ControllerBase
{

    private readonly ILessonService _service;
    private readonly IQuestionService _questionService;
    private readonly ILessonProgressService _lessonProgressService;

    public LessonsController(ILessonService service, IQuestionService questionService, ILessonProgressService lessonProgressService)
    {
        _service = service;
        _questionService = questionService;
        _lessonProgressService = lessonProgressService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] LessonRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.CreateLessonAsync(request, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return BadRequest(serviceResponse);
        }

        return Created($"/api/lessons/{serviceResponse.Data}", serviceResponse);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] LessonRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.UpdateLessonAsync(id, request, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return BadRequest(serviceResponse);
        }
        
        return Ok(serviceResponse);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.DeleteLessonAsync(id, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return NotFound(serviceResponse);
        }
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetLessonByIdAsync(id, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return NotFound(serviceResponse);
        }
        
        return Ok(serviceResponse);
    }

    [HttpGet("{lessonId:guid}/questions")]
    public async Task<IActionResult> GetQuestionsByLessonIdAsync(
        [FromRoute] Guid lessonId,
        [FromQuery] GetQuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _questionService.GetQuestionsByLessonIdAsync(lessonId, request, cancellationToken);
        if (!serviceResponse.Succeeded) 
            return NotFound(serviceResponse);
        
        return Ok(serviceResponse);
    }
    [HttpGet("{lessonId:guid}/lesson-progress")]
    [Authorize]
    public async Task<IActionResult> GetLessonProgress(
        [FromRoute] Guid lessonId,
        [FromQuery] GetLessonProgressByLessonIdRequest request, 
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new Exception("User not found");

        var result = await _lessonProgressService
            .GetLessonProgressByLessonIdAsync(userId, lessonId, request, cancellationToken);
        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
}