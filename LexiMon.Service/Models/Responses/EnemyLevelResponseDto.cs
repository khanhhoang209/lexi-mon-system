namespace LexiMon.Service.Models.Responses;

public class EnemyLevelResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int FromLevel { get; set; }
    public int ToLevel { get; set; }
    public bool IsActive { get; set; }
}