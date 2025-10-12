using LexiMon.Repository.Enum;

namespace LexiMon.Service.Models.Responses;

public class UserResponseDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? FirstName { get; set; } = null;
    public string? LastName { get; set; } = null;
    public string? Address { get; set; } = null;
    public DateTimeOffset? BirthDate { get; set; }
    public decimal Coins { get; set; }
    public Gender Gender { get; set; }
}