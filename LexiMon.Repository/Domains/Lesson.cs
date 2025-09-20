using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class Lesson : BaseAuditableEntity<Guid>
{
    public string? Description { get; set; }
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public LessonProgress? LessonProgress { get; set; }
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}