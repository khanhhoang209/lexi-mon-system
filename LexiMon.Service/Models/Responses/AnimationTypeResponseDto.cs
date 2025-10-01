namespace LexiMon.Service.Models.Responses;

public class AnimationTypeResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive {get; set;}
    public DateTimeOffset CreatedAt { get; set; }
}