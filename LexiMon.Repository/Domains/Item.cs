using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class Item : BaseAuditableEntity<Guid>
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public Decimal? Price { get; set; }
    public Decimal? Coin { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public Category? Category { get; set; } = null;
    public Order? Order { get; set; } = null;
    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    public ICollection<Animation> Animations { get; set; } = new List<Animation>();
}