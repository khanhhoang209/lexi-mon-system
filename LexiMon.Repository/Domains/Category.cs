using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class Category : BaseAuditableEntity<Guid>
{
    public string? Name { get; set; }
    public Item? Item { get; set; } = null;
}