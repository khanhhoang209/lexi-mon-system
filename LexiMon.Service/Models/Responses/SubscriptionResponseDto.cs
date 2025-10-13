namespace LexiMon.Service.Models.Responses;

public class SubscriptionResponseDto
{
    public int Free { get; set; }
    public int Premium { get; set; }
    public int Total => Free + Premium;
}