using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class Achievement : BaseAuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int TargetValue { get; set; }
    public int RewardExps { get; set; }
    public int RewardCoins { get; set; }
    public bool IsRepeatable { get; set; } = false;
    public ICollection<AchievementUser> AchievementUsers { get; set; } = new List<AchievementUser>();
}