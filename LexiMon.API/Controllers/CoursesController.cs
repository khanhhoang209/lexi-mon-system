using System.Security.Claims;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;
    private readonly ILessonService _lessonService;
    private readonly IAzureBlobService _blob;
    private readonly string _defaultContainer;

    public CoursesController(ICourseService service, ILessonService lessonService, IAzureBlobService blob, IConfiguration configuration)
    {
        _service = service;
        _lessonService = lessonService;
        _blob = blob;
        _defaultContainer = configuration["Azure:BlobStorageSettings:DefaultContainer"] ?? "images";
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> CreateAsync(
        [FromForm] CourseCreateFormDto request,
        CancellationToken cancellationToken = default)
    {
        string? imageUrl = null;
        if (request.Image is { Length: > 0 })
        {
            using var s = request.Image.OpenReadStream();
            imageUrl = await _blob.UploadAsync(s, request.Image.FileName, _defaultContainer);
        }

        var requestDto = new CourseRequestDto
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Coin = request.Coin,
            ImageUrl = imageUrl
        };

        var serviceResponse = await _service.CreateCourseAsync(requestDto, cancellationToken);
        if (!serviceResponse.Succeeded) return BadRequest(serviceResponse);

        return Created($"/api/courses/{serviceResponse.Data}", serviceResponse);
    }

    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromForm] CourseCreateFormDto request,
        CancellationToken cancellationToken = default)
    {
        
        string? imageUrl = null;
        if (request.Image is { Length: > 0 })
        {
            using var s = request.Image.OpenReadStream();
            imageUrl = await _blob.UploadAsync(s, request.Image.FileName, _defaultContainer);
        }

        var requestDto = new CourseRequestDto
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Coin = request.Coin,
            ImageUrl = imageUrl
        };
        
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
    [HttpGet("shop")]
    [Authorize]
    public async Task<IActionResult> GetShopCoursesAsync(
        [FromQuery] GetCourseRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new Exception("User not found");
        var serviceResponse = await _service.GetShopCoursesAsync(userId ,request, cancellationToken);
        return Ok(serviceResponse);
    }
}

