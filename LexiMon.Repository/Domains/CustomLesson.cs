using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class CustomLesson : BaseAuditableEntity<Guid>
{
    public string? Description { get; set; }
    
    public Guid UserDeckId { get; set; }
    // public UserDeck UserDeck { get; set; } = null!;
    
    public LessonProgress? LessonProgress { get; set; }
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}