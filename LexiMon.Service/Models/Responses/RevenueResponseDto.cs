namespace LexiMon.Service.Models.Responses;

public class RevenueResponseDto
{
    public decimal ItemRevenue { get; set; }
    public decimal CourseRevenue { get; set; }
    public decimal PremiumRevenue { get; set; }
    public decimal TotalRevenue => ItemRevenue + CourseRevenue + PremiumRevenue;
}