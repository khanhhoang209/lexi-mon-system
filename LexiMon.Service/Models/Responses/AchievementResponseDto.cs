namespace LexiMon.Service.Models.Responses;

public class AchievementResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int TargetValue { get; set; }
    public int RewardExps { get; set; }
    public int RewardCoins { get; set; }
    public bool IsRepeatable { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsActive { get; set; }
}