using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LexiMon.Service.Models.Requests;

public class ItemRequestDto
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsPremium { get; set; } = false;
    public Decimal? Price { get; set; }
    public Decimal? Coin { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
}

public class ItemFormDto
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsPremium { get; set; } = false;
    [Range(0, double.MaxValue, ErrorMessage = "Giá lớn hơn hoặc bằng 0!")]
    public Decimal? Price { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Số xu lớn hơn hoặc bằng 0!")]
    public Decimal? Coin { get; set; }
    public IFormFile? Image { get; set; }
    public string? Description { get; set; }
}