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

    public int TotalCount { get; set; }
    public int CorrectCount { get; set; }
    public string? LessonName { get; set; }
    public string? CustomLessonTitle { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
}

public class GetLessonProgressByLessonIdRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; init; } = 1;
    
    [Range(1, int.MaxValue, ErrorMessage = "PageSize must be greater than 0")]
    public int PageSize { get; init; } = 8;

    public LessonProgressStatus? LessonProgressStatus { get; init; } = null;

    public int TotalCount { get; set; }
    
    public int CorrectCount { get; set; }
    
    public string? Title { get; set; }
}