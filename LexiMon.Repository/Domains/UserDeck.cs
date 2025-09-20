using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class UserDeck : BaseAuditableEntity<Guid>
{
    public string UserId { get; set; }
    public Guid? CourseId { get; set; } = null!;
    public Guid? CustomLessonId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public Course? Course { get; set; } = null!;
    public ICollection<CustomLesson> CustomLessons { get; set; } = new List<CustomLesson>();
}