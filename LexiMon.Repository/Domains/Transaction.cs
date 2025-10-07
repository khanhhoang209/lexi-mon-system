using LexiMon.Repository.Common;
using LexiMon.Repository.Enum;

namespace LexiMon.Repository.Domains;

public class Transaction : BaseAuditableEntity<Guid>
{
    public Guid OrderId { get; set; } = Guid.Empty;
    public long OrderCode { get; set; }
    public string PaymentLinkId { get; set; } = null!;
    public string CheckoutUrl { get; set; } = null!;
    public string QrCode { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Description { get; set; } = string.Empty;
    public DateTimeOffset TransactionDate { get; set; } = DateTimeOffset.UtcNow;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.PayOs;
    public TransactionStatus TransactionStatus { get; set; } = TransactionStatus.Cancel;
}