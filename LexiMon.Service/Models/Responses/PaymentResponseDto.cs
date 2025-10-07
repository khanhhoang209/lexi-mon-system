namespace LexiMon.Service.Models.Responses;

public class PaymentResponseDto
{
    public Guid Id { get; set; }
    public string AccountNumber { get; set; } = null!;
}