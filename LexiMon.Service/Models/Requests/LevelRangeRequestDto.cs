namespace LexiMon.Service.Models.Requests;

public class LevelRangeRequestDto
{
    public string Name { get; set; } = null!;
    public int FromExp { get; set; }
    public int ToExp { get; set; }
}