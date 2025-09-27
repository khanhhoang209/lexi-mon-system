using LexiMon.Repository.Domains;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Mappers;

public static class CustomLessonMapper
{
    public static CustomLessonResponseDto ToCustomLessonResponse(this CustomLesson l)
    {
        return new CustomLessonResponseDto()
        {
            CustomLessonId = l.Id,
            Title = l.Title,
            Description = l.Description,
            IsActive = l.Status,
            CreatedAt = l.CreatedAt,
        };
    }
    
    public static CustomLesson ToLesson(this CustomLessonRequestDto request)
    {
        return new CustomLesson()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
        };
    }

    public static void UpdateCustomLesson(this CustomLesson lesson, CustomLessonRequestDto request)
    {
        lesson.Title = request.Title;
        lesson.Description = request.Description;
    }
}