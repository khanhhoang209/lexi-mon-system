using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class Enemy : BaseAuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? AnimationAttackUrl { get; set; }
    public string? AnimationMoveUrl { get; set; }
    public string? HelmerUrl { get; set; }
    public string? ArmorUrl { get; set; }
    public string? BootUrl { get; set; }
    public string? WeaponUrl { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public Guid EnemyLevelId { get; set; }
    public EnemyLevel? EnemyLevel { get; set; }
}