using LexiMon.Repository.Enum;

namespace LexiMon.Service.Models.Requests;

public class UserRequestDto
{
    public string? FirstName { get; set; } = null;
    public string? LastName { get; set; } = null;
    public string? Address { get; set; } = null;
    public DateTimeOffset? BirthDate { get; set; }
    public Gender Gender { get; set; }
}