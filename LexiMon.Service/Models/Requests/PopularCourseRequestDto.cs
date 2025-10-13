namespace LexiMon.Service.Models.Requests;

public class PopularLanguageRequestDto
{
    public DateTimeOffset? StartDate { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset? EndDate { get; set; } = DateTimeOffset.MaxValue;
}