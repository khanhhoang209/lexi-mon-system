using LexiMon.Repository.Common;
using LexiMon.Repository.Enum;

namespace LexiMon.Repository.Domains;

public class LessonProgress : BaseAuditableEntity<Guid>
{
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public int CorrectCount  { get; set; }
    public int TotalCount  { get; set; }
    public int? TimeSpentSeconds { get; set; }
    public LessonProgressStatus LessonProgressStatus {get; set;}
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public Guid? LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public Guid? CustomLessonId { get; set; }
    public CustomLesson? CustomLesson { get; set; }
}