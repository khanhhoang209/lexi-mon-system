using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class Character : BaseAuditableEntity<Guid>
{
    public string UserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Level { get; set; }
    public int Exp { get; set; } = 0;
    public string? HelmetUrl { get; set; } = null!;
    public string? ArmorUrl { get; set; } = null!;
    public string? WeaponUrl { get; set; } = null!;
    public string? BootUrl { get; set; } = null!;
    public ApplicationUser? User { get; set; } = null;
    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
}