using System.ComponentModel.DataAnnotations;
using LexiMon.Repository.Domains;
using LexiMon.Repository.Enum;

namespace LexiMon.Service.Models.Requests;

public class LessonProgressRequestDto
{
    [Range(0, int.MaxValue, ErrorMessage = "TargetValue phải lớn hơn hoặc bằng 0")]
    public int TargetValue { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "CurrentValue phải lớn hơn hoặc bằng 0")]
    public int CurrentValue { get; set; }
    public LessonProgressStatus LessonProgressStatus {get; set;}
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public Guid? LessonId { get; set; }
    public Guid? CustomLessonId { get; set; }
}