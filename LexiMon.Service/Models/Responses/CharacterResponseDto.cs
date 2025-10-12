namespace LexiMon.Service.Models.Responses;

public class CharacterResponseDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Level { get; set; }
    public int Exp { get; set; } = 0;
    public string? HelmetUrl { get; set; } = null!;
    public string? ArmorUrl { get; set; } = null!;
    public string? WeaponUrl { get; set; } = null!;
    public string? BootUrl { get; set; } = null!;
}