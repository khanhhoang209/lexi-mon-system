using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class UserDeck : BaseAuditableEntity<Guid>
{
    public Guid UserId { get; set; }
    public Guid? CourseId { get; set; } = null!;
    public Guid? CustomLessonId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}