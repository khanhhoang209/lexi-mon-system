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
            CorrectCount = lp.CorrectCount,
            TotalCount = lp.TotalCount,
            TimeSpentFormatted = FormatTimeSpent(lp.TimeSpentSeconds),
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
            CorrectCount = request.CorrectCount,
            TotalCount = request.TotalCount,
            TimeSpentSeconds = request.TimeSpentSeconds,
            LessonProgressStatus = LessonProgressStatus.InProgress,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            LessonId = request.LessonId,
            CustomLessonId = request.CustomLessonId,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = true,
        };
    }

    public static void UpdateLessonProgress(this LessonProgress lp, LessonProgressRequestDto request)
    {
        lp.CorrectCount = request.CorrectCount;
        lp.TotalCount = request.TotalCount;
        lp.TimeSpentSeconds = request.TimeSpentSeconds;
        lp.LessonProgressStatus = request.LessonProgressStatus;
        lp.EndDate = request.EndDate;
        lp.StartDate = request.StartDate;
        lp.UpdatedAt = DateTimeOffset.UtcNow;
    }
    public static string FormatTimeSpent(int? seconds)
    {
        if (seconds is null || seconds <= 0)
            return "00:00";

        TimeSpan time = TimeSpan.FromSeconds(seconds.Value);
        return $"{(int)time.TotalMinutes:D2}:{time.Seconds:D2}";
    }

}