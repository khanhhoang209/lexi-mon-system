namespace LexiMon.Service.Models.Responses;

public class CustomLessonResponseDto
{
    public Guid CustomLessonId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}