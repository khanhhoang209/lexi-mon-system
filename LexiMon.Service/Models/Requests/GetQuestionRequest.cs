using System.ComponentModel.DataAnnotations;

namespace LexiMon.Service.Models.Requests;

public class GetQuestionRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;          
    [Range(1, int.MaxValue, ErrorMessage = "Pagesize must be greater than 0")]
    public int PageSize { get; set; } = 8;   
    public string QuestionContent {get; set;} = string.Empty;
    public bool? IsActive { get; init; }
}