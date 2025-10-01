using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly IAzureBlobService _blob;

    public FilesController(IAzureBlobService blob) => _blob = blob;

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)] //50mb
    public async Task<IActionResult> Upload([FromForm] FileUploadDto model)
    {
        if (model.File == null || model.File.Length == 0) return BadRequest("No file");

        using var stream = model.File.OpenReadStream();

        var url = await _blob.UploadAsync(stream, model.File.FileName, "images");

        return Ok(new { url });
    }


}