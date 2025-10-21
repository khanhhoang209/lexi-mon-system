using LexiMon.Repository.Enum;

namespace LexiMon.Service.Models.Responses;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? ItemId { get; set; }
    public Decimal? PurchaseCost { get; set; }
    public Decimal? CoinCost { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? ItemName { get; set; }
    public string? CourseTitle { get; set; }
    public string Email { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}