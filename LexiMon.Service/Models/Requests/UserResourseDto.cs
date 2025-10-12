using System.ComponentModel.DataAnnotations;

namespace LexiMon.Service.Models.Requests;

public class UserResourseDto
{
    public decimal? Coins { get; set; } = 0;
    [Range(0, int.MaxValue, ErrorMessage = "Exp phải là số nguyên dương hoặc bằng 0")]
    public int? Exp { get; set; } = 0;
}