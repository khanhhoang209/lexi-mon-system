using Microsoft.AspNetCore.Http;

namespace LexiMon.Service.Models.Requests;

public class CharacterRequestDto
{
    public string Name { get; set; } = null!;
    public int Level { get; set; }
    public int Exp { get; set; } = 0;
    public string? HelmetUrl { get; set; } = null!;
    public string? ArmorUrl { get; set; } = null!;
    public string? WeaponUrl { get; set; } = null!;
    public string? BootUrl { get; set; } = null!;
}

public class CharacterFormDto
{
    public string Name { get; set; } = null!;
    public int Level { get; set; }
    public int Exp { get; set; } = 0;
    public IFormFile? Helmet { get; set; } = null!;
    public IFormFile? Armor { get; set; } = null!;
    public IFormFile? Weapon { get; set; } = null!;
    public IFormFile? Boot { get; set; } = null!;
}