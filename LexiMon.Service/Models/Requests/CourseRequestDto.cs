using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace LexiMon.Service.Models.Requests;

public class CourseRequestDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public decimal? Price { get; set; }
    public decimal? Coin {get; set;}
    public Guid CourseLanguageId { get; set; }
}

public class CourseCreateFormDto
{
    [Required(ErrorMessage = "Vui lòng nhập tiêu đề!")]
    [MaxLength(300, ErrorMessage = "Tiêu đề không được vượt quá 300 ký tự!")]
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    [Range(0, double.MaxValue, ErrorMessage = "Giá lớn hơn hoặc bằng 0!")]
    public decimal? Price { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Số xu lớn hơn hoặc bằng 0!")]
    public decimal? Coin { get; set; }
    public IFormFile? Image { get; set; }
    public Guid CourseLanguageId { get; set; }
}
