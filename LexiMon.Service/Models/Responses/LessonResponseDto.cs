namespace LexiMon.Service.Models.Responses;

public class LessonResponseDto
{
    public Guid LessonId { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    
    //    public int QuestionCount { get; set; }
    
}