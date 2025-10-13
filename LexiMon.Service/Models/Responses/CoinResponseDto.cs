namespace LexiMon.Service.Models.Responses;

public class CoinResponseDto
{
    public decimal ItemCoin { get; set; }
    public decimal CourseCoin { get; set; }
    public decimal TotalCoin => ItemCoin + CourseCoin;
}