namespace LexiMon.Service.Models.Responses;

public class AnimationResponseDto
{
    public Guid AnimationId { get; set; }
    public string? AnimationName { get; set; }
    public Guid? ItemId { get; set; }
    public string? ItemName { get; set; }
    public Guid? AnimationTypeId { get; set; }
    public string? AnimationTypeName { get; set; }
    public string? AnimationUrl { get; set; }
    public bool IsActive { get; set; }
}