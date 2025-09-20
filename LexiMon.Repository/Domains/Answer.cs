using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class Answer : BaseAuditableEntity<Guid>
{
    public string Content { get; set; } = null!;
    public bool IsCorrect { get; set; }
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;
}