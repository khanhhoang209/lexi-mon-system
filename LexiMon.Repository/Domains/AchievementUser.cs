using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class AchievementUser : BaseAuditableEntity<(Guid UserId, Guid AchievementId)>
{
    public DateTimeOffset AchievedAt { get; set; }
    public int Progress { get; set; }
    public ApplicationUser? User { get; set; } = null;
    public Achievement? Achievement { get; set; } = null;
}