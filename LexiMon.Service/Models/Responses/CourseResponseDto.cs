namespace LexiMon.Service.Models.Responses;

public class CourseResponseDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public decimal? Price { get; set; }
    public decimal? Coin {get; set;}
    public bool IsActive { get; set; }
    public Guid CourseLanguageId { get; set; }
    public string CourseLanguageName { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

}