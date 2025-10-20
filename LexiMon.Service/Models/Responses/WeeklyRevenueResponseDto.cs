namespace LexiMon.Service.Models.Responses;

public class WeeklyRevenueResponseDto
{
    public RevenueResponseDto Monday { get; set; } = null!;
    public RevenueResponseDto Tuesday { get; set; } = null!;
    public RevenueResponseDto Wednesday { get; set; } = null!;
    public RevenueResponseDto Thursday { get; set; } = null!;
    public RevenueResponseDto Friday { get; set; } = null!;
    public RevenueResponseDto Saturday { get; set; } = null!;
    public RevenueResponseDto Sunday { get; set; } = null!;
}