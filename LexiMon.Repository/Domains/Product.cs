using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class Product : BaseAuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
}