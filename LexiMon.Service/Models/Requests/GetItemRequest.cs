using System.ComponentModel.DataAnnotations;

namespace LexiMon.Service.Models.Requests;

public class GetItemRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; init; } = 1;
    
    [Range(1, int.MaxValue, ErrorMessage = "PageSize must be greater than 0")]
    public int PageSize { get; init; } = 8;
    public string ItemName { get; init; } = string.Empty;
    public string? CategoryName { get; init; } = string.Empty;
    public decimal? MinPrice { get; init; } 
    public decimal? MaxPrice { get; init; } 
}