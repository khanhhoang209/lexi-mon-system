using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;
    private readonly ILessonService _lessonService;

    public CoursesController(ICourseService service, ILessonService lessonService)
    {
        _service = service;
        _lessonService = lessonService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CourseRequestDto requestDto,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.CreateCourseAsync(requestDto, cancellationToken);
        if (!serviceResponse.Succeeded)
            return BadRequest(serviceResponse);

        return Created($"/api/courses/{serviceResponse.Data}", serviceResponse);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] CourseRequestDto requestDto,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.UpdateCourseAsync(id, requestDto, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return BadRequest(serviceResponse);
        }

        return Ok(serviceResponse);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.DeleteCourseAsync(id, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return NotFound(serviceResponse);
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetCoursesAsync(
        [FromQuery] GetCourseRequest request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetCoursesAsync(request, cancellationToken);
        return Ok(serviceResponse);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetCourseByIdAsync(id, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return NotFound(serviceResponse);
        }

        return Ok(serviceResponse);
    }

    [HttpGet("{courseId:guid}/lessons")]
    public async Task<IActionResult> GetLessonsByCourseIdAsync(
        [FromRoute] Guid courseId,
        [FromQuery] GetLessonRequest request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _lessonService.GetLessonsByCourseIdAsync(courseId, request, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return NotFound(serviceResponse);
        }

        return Ok(serviceResponse);
    }
}

