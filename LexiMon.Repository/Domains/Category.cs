using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class Category : BaseAuditableEntity<Guid>
{
    public string? Name { get; set; }
    public ICollection<Item> Items { get; set; } = new List<Item>();
}