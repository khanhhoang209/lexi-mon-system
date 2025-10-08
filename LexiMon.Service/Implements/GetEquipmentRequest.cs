using System.ComponentModel.DataAnnotations;

namespace LexiMon.Service.Implements;

public class GetEquipmentRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;          
    [Range(1, int.MaxValue, ErrorMessage = "Pagesize must be greater than 0")]
    public int PageSize { get; set; } = 8;     
    public string? ItemName { get; set; }   
    public string? CategoryName { get; set; }
    public bool? IsPremium { get; set; }
}