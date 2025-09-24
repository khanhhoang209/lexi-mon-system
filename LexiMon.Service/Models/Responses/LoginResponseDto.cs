namespace LexiMon.Service.Models.Responses;

public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public int ExpiredIn { get; set; } = 0;
}