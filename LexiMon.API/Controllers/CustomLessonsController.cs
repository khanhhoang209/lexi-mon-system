using System.Security.Claims;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/custom-lessons")]
public class CustomLessonsController : ControllerBase
{
    private readonly ICustomLessonService _service;
    private readonly IQuestionService _questionService;
    private readonly ILessonProgressService _lessonProgressService;

    public CustomLessonsController(ICustomLessonService service, IQuestionService questionService, ILessonProgressService lessonProgressService)
    {
        _service = service;
        _questionService = questionService;
        _lessonProgressService = lessonProgressService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CustomLessonRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var userId= User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found");
        var serviceResponse = await _service.CreateCustomLessonAsync(request, userId, cancellationToken);
        if (!serviceResponse.Succeeded)
            return BadRequest(serviceResponse);
        
        return Created($"/api/custom-lessons/{serviceResponse.Data}", serviceResponse);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] CustomLessonRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.UpdateCustomLessonAsync(id, request, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return BadRequest(serviceResponse);
        }

        return Ok(serviceResponse);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.DeleteCustomLessonAsync(id, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return NotFound(serviceResponse);
        }

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetCustomLessonByIdAsync(id, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return NotFound(serviceResponse);
        }

        return Ok(serviceResponse);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var userId= User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found");
        var serviceResponse = await _service.GetCustomLessonsAsync(userId, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return Ok(serviceResponse);
        }
        return BadRequest(serviceResponse);
    }

    [HttpGet("{customLessonId}/questions")]
    [Authorize]
    public async Task<IActionResult> GetQuestionsByCustomLessonIdAsync(
        [FromRoute] Guid customLessonId,
        [FromQuery] GetQuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _questionService.GetQuestionsByCustomLessonIdAsync(customLessonId, request, cancellationToken);
        if (!serviceResponse.Succeeded)
            return NotFound(serviceResponse);
        
        return Ok(serviceResponse);
    }
    [HttpGet("{customLessonId}/lesson-progress")]
    [Authorize]
    public async Task<IActionResult> GetLessonProgress(
        [FromRoute] Guid customLessonId,
        [FromQuery] GetLessonProgressByLessonIdRequest request, 
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new Exception("User not found");

        var result = await _lessonProgressService
            .GetLessonProgressByCustomLessonAsync(userId, customLessonId, request, cancellationToken);
        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
}