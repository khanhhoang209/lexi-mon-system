namespace LexiMon.Service.Models.Responses;

public class LevelRangeResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int FromExp { get; set; }
    public int ToExp { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}