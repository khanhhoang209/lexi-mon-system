using Microsoft.AspNetCore.Http;

namespace LexiMon.Service.Models.Requests;

public class AnimationRequestDto
{
    public string? Name { get; set; }
    public Guid ItemId { get; set; }
    public Guid AnimationTypeId { get; set; }
    public string? AnimationUrl { get; set; }
}
public class AnimationFormDto
{
    public string? Name { get; set; }
    public Guid ItemId { get; set; }
    public Guid AnimationTypeId { get; set; }
    public IFormFile? Image { get; set; }
}