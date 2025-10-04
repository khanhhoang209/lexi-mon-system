namespace LexiMon.Service.Models.Responses;

public class ItemResponseDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; } = null!;
    public Decimal? Price { get; set; }
    public Decimal? Coin { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public bool IsActive {get;set;}
    
}