namespace LexiMon.Service.Models.Requests;

public class AchievementRequestDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int TargetValue { get; set; }
    public int RewardExps { get; set; }
    public int RewardCoins { get; set; }
    public bool IsRepeatable { get; set; } = false;
}