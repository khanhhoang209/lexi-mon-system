using System.ComponentModel.DataAnnotations;

namespace LexiMon.Service.Models.Requests;

public class GetAnimationTypeRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; init; } = 1;
    
    [Range(1, int.MaxValue, ErrorMessage = "PageSize must be greater than 0")]
    public int PageSize { get; init; } = 8;
    
    public string AnimationTypeName {get; init;} = string.Empty;
}