using LexiMon.Repository.Common;
using LexiMon.Repository.Enum;

namespace LexiMon.Repository.Domains;

public class Order : BaseAuditableEntity<Guid>
{
    public Guid UserId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? ItemId { get; set; }
    public Decimal PurchaseCost { get; set; }
    public Decimal CoinCost { get; set; }
    public DateTime PaidAt { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public ApplicationUser? User { get; set; } = null!;
    public Item? Item { get; set; } = null!;
}