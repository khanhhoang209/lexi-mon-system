using System.ComponentModel.DataAnnotations;

namespace LexiMon.Service.Models.Requests;

public class GetLessonRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;          
    [Range(1, int.MaxValue, ErrorMessage = "Pagesize must be greater than 0")]
    public int PageSize { get; set; } = 8;     
    public string? Title { get; set; }   
    public bool? IsActive { get; init; }
}