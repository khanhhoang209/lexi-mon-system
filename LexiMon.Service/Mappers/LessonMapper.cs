using LexiMon.Repository.Domains;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Mappers;

public static class LessonMapper
{
    public static LessonResponseDto ToLessonResponse(this Lesson l)
    {
        return new LessonResponseDto()
        {
            LessonId = l.Id,
            CourseId = l.CourseId, 
            Title = l.Title,
            Description = l.Description,
            IsActive = l.Status,
            CreatedAt = l.CreatedAt,
            
        };
    }
    
    public static Lesson ToLesson(this LessonRequestDto request)
    {
        return new Lesson()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CourseId = request.CourseId,
            Status = true
        };
    }

    public static void UpdateLesson(this Lesson lesson, LessonRequestDto request)
    {
        lesson.Title = request.Title;
        lesson.Description = request.Description;
        lesson.CourseId = request.CourseId;
    }
}