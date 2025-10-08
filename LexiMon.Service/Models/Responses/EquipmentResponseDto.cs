namespace LexiMon.Service.Models.Responses;

public class EquipmentResponseDto
{
    public Guid CharacterId { get; set; }
    public Guid ItemId { get; set; }
    public string? ItemName { get; set; }
    public string? ImageUrl { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool IsPremium { get; set; }
}