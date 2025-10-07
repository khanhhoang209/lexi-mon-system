using LexiMon.Repository.Common;
using LexiMon.Repository.Enum;

namespace LexiMon.Repository.Domains;

public class Order : BaseAuditableEntity<Guid>
{
    public string UserId { get; set; } = null!;
    public Guid? CourseId { get; set; }
    public Guid? ItemId { get; set; }
    public Decimal? PurchaseCost { get; set; }
    public Decimal? CoinCost { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public Item? Item { get; set; } = null!;
    public Course? Course { get; set; } = null!;
}