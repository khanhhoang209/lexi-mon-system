using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class AnimationType : BaseAuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
    public ICollection<Animation> Animations { get; set; } = new List<Animation>();
}