using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class Equipment : BaseAuditableEntity<Guid>
{
    public Guid CharacterId { get; set; }
    public Guid ItemId { get; set; }
    public Character? Character { get; set; } = null;
    public Item? Item { get; set; } = null;
}