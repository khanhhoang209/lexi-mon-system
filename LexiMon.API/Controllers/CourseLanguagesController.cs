using LexiMon.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/course-languages")]
public class CourseLanguagesController : ControllerBase
{
    private readonly ICourseLanguageService _service;

    public CourseLanguagesController(ICourseLanguageService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourseLanguages(CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetCourseLanguages(cancellationToken);
        if (!serviceResponse.Succeeded) return BadRequest(serviceResponse);

        return Ok(serviceResponse);
    }
    
}