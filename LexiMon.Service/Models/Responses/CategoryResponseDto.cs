namespace LexiMon.Service.Models.Responses;

public class CategoryResponseDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; } = null;
    public bool IsActive {get; set;}
    public DateTimeOffset CreateAt {get; set;}
}