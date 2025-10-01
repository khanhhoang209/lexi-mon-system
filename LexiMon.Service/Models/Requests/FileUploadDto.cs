using Microsoft.AspNetCore.Http;

namespace LexiMon.Service.Models.Requests;

public class FileUploadDto
{
    public IFormFile File { get; set; } = default!;
}