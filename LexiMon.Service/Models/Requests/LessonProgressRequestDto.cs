using LexiMon.Repository.Domains;
using LexiMon.Repository.Enum;

namespace LexiMon.Service.Models.Requests;

public class LessonProgressRequestDto
{
    public int TargetValue { get; set; }
    public int CurrentValue { get; set; }
    public LessonProgressStatus LessonProgressStatus {get; set;}
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public Guid? LessonId { get; set; }
    public Guid? CustomLessonId { get; set; }
}