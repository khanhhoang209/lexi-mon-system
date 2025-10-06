using Microsoft.AspNetCore.Http;

namespace LexiMon.Service.Models.Requests;

public class EnemyRequestDto
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
}

public class EnemyFormDto
{
    public string Name { get; set; } = null!;
    public IFormFile? ImageUrl { get; set; }
    public IFormFile? AnimationAttackUrl { get; set; }
    public IFormFile? AnimationMoveUrl { get; set; }
    public IFormFile? HelmerUrl { get; set; }
    public IFormFile? ArmorUrl { get; set; }
    public IFormFile? BootUrl { get; set; }
    public IFormFile? WeaponUrl { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public Guid EnemyLevelId { get; set; }
}