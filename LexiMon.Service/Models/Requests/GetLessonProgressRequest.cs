using System.ComponentModel.DataAnnotations;
using LexiMon.Repository.Enum;

namespace LexiMon.Service.Models.Requests;

public class GetLessonProgressRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; init; } = 1;
    
    [Range(1, int.MaxValue, ErrorMessage = "PageSize must be greater than 0")]
    public int PageSize { get; init; } = 8;

    public LessonProgressStatus? LessonProgressStatus { get; init; } = null;

    public int TargetValue { get; set; }
    
    public int CurrentValue { get; set; }
    
    public string? LessonName { get; set; }
    public string? CustomLessonTitle { get; set; }
}