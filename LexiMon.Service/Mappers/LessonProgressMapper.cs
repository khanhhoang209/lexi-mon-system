using LexiMon.Repository.Domains;
using LexiMon.Repository.Enum;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Mappers;

public static class LessonProgressMapper
{
    public static LessonProgressResponseDto ToLessonProgressResponse(this LessonProgress lp)
    {
        return new LessonProgressResponseDto()
        {
            LessonProgressId = lp.Id,
            CurrentValue = lp.CurrentValue,
            TargetValue = lp.TargetValue,
            LessonProgressStatus = lp.LessonProgressStatus,
            StartDate = lp.StartDate,
            EndDate = lp.EndDate,
            CustomLessonId = lp.CustomLessonId,
            CustomLessonTitle = lp.CustomLesson != null ? lp.CustomLesson.Title : null,
            LessonId = lp.LessonId,
            LessonName = lp.Lesson != null ? lp.Lesson.Title : null,
            IsActive = lp.Status
        };
    }

    public static LessonProgress ToLessonProgress(this LessonProgressRequestDto request, string userId)
    {
        return new LessonProgress()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TargetValue = request.TargetValue,
            CurrentValue = request.CurrentValue,
            LessonProgressStatus = LessonProgressStatus.InProgress,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            LessonId = request.LessonId,
            CustomLessonId = request.CustomLessonId,
            CreatedAt = DateTimeOffset.Now,
            Status = true
        };
    }

    public static void UpdateLessonProgress(this LessonProgress lp, LessonProgressRequestDto request)
    {
        lp.TargetValue = request.TargetValue;
        lp.CurrentValue = request.CurrentValue;
        lp.LessonProgressStatus = request.LessonProgressStatus;
        lp.EndDate = request.EndDate;
        lp.StartDate = request.StartDate;
        lp.UpdatedAt = DateTimeOffset.Now;
    }
}