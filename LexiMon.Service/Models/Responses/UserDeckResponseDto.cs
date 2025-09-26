namespace LexiMon.Service.Models.Responses;

public class UserDeckResponseDto
{
    public Guid UserDeckId { get; set; }
    public Guid? CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public string? CourseDescription { get; set; }
    public string? CourseImageUrl { get; set; }
    public Guid? CustomLessonId { get; set; }
    public string? CustomLessonTitle { get; set; }
    public string? CustomLessonDescription { get; set; }
    public bool IsActive { get; set; }
}