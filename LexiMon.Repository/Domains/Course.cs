using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class Course : BaseAuditableEntity<Guid>
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public decimal? Price { get; set; }
    public decimal? Coin {get; set;}
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    // public ICollection<UserDeck> UserDecks { get; set; } = new List<UserDeck>();
}