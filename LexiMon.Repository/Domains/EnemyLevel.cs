using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class EnemyLevel : BaseAuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
    public int FromLevel { get; set; }
    public int ToLevel { get; set; }
    public Enemy? Enemy { get; set; }
}