namespace LexiMon.Service.Models.Requests;

public class EnemyLevelRequestDto
{
    public string Name { get; set; } = null!;
    public int FromLevel { get; set; }
    public int ToLevel { get; set; }
}