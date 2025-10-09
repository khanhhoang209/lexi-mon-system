using LexiMon.Repository.Enum;

namespace LexiMon.Service.Models.Responses;

public class LessonProgressResponseDto
{
    public Guid LessonProgressId {get; set;}
    public int CorrectCount  { get; set; }
    public int TotalCount  { get; set; }
    
    public string? TimeSpentFormatted { get; set; }
    public LessonProgressStatus LessonProgressStatus {get; set;}
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public Guid? LessonId { get; set; }
    public string? LessonName { get; set; }
    public Guid? CustomLessonId { get; set; }
    
    public string? CustomLessonTitle { get; set; }
    public bool IsActive { get; set; }
}