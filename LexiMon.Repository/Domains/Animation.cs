using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class Animation : BaseAuditableEntity<Guid>
{
    public string? Name { get; set; }
    public Guid ItemId { get; set; }
    public Guid AnimationTypeId { get; set; }
    public string? AnimationUrl { get; set; }
    public AnimationType? AnimationType { get; set; }
    public Item? Item { get; set; }
}