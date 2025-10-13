using System.ComponentModel.DataAnnotations;

namespace LexiMon.Service.Models.Requests;

public class GetUserDeckRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; init; } = 1;
    
    [Range(1, int.MaxValue, ErrorMessage = "PageSize must be greater than 0")]
    public int PageSize { get; init; } = 8;
    public string CourseTitle { get; init; } = string.Empty;
    public string CustomLessonTitle {get; set; } = string.Empty;
    public bool? IsActive { get; init; }
}