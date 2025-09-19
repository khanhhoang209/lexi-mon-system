using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class LevelRange : BaseAuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
    public int FromExp { get; set; }
    public int ToExp { get; set; }
}