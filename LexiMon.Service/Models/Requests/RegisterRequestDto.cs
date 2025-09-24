using System.ComponentModel.DataAnnotations;
using LexiMon.Repository.Enum;

namespace LexiMon.Service.Models.Requests;

public class RegisterRequestDto
{
    [EmailAddress(ErrorMessage = "Vui lòng nhập email hợp lệ!")]
    [Required(ErrorMessage = "Vui lòng nhập email!")]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
    public string Password { get; set; } = null!;
    public string? FirstName { get; set; } = null!;
    public string? LastName { get; set; } = null!;
    public string? Address { get; set; } = null!;
    public DateTimeOffset? BirthDate { get; set; }
    public Gender? Gender { get; set; } = Repository.Enum.Gender.Other;
}