using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class LessonProgress : BaseAuditableEntity<Guid>
{

    public int TargetValue { get; set; }
    public int CurrentValue { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    
    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
    
    public Guid? CustomLessonId { get; set; }
    public CustomLesson? CustomLesson { get; set; }
    //public Order? Order { get; set; }
}