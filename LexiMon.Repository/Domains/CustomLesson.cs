using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class CustomLesson : BaseAuditableEntity<Guid>
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<UserDeck> UserDeck { get; set; } = new List<UserDeck>();
    public LessonProgress? LessonProgress { get; set; }
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}