using System.ComponentModel.DataAnnotations;
using LexiMon.Repository.Enum;

namespace LexiMon.Service.Models.Requests;

public class GetOrderUserRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;          
    [Range(1, int.MaxValue, ErrorMessage = "Pagesize must be greater than 0")]
    public int PageSize { get; set; } = 8;     
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public PaymentStatus? PaymentStatus { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
}