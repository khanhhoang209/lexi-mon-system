namespace LexiMon.Service.Models.Responses;

public class EnemyResponseDto
{
    public Guid EnemyId { get; set; }
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
    public bool IsActive { get; set; }
    public Guid EnemyLevelId { get; set; }
    public string EnemyLevelName { get; set; } = null!;
    
}