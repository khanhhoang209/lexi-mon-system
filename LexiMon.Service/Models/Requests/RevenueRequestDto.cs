namespace LexiMon.Service.Models.Requests;

public class RevenueRequestDto
{
    public DateTimeOffset? StartDate { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset? EndDate { get; set; } = DateTimeOffset.MaxValue;
}