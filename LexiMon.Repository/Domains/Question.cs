using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class Question : BaseAuditableEntity<Guid>
{
    public string Content { get; set; } = null!;
    public Guid? LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public Guid? CustomLessonId { get; set; }
    public CustomLesson? CustomLesson { get; set; }
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}