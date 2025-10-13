using LexiMon.Repository.Domains;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Mappers;

public static class CourseMapper
{
    public static CourseResponseDto ToCourseResponse(this Course course)
    {
        return new CourseResponseDto()
        {
            CourseId = course.Id,
            Title = course.Title,
            Description = course.Description,
            ImageUrl = course.ImageUrl,
            Price = course.Price,
            Coin = course.Coin,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt,
            IsActive = course.Status,
            CourseLanguageId = course.CourseLanguageId,
            CourseLanguageName = course.CourseLanguage!.Name
        };
    }

    public static Course ToCourse(this CourseRequestDto request)
    {
        return new Course()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Price = request.Price,
            Coin = request.Coin,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = true,
            CourseLanguageId = request.CourseLanguageId,
        };
    }

    public static void UpdateCourse(this Course course, CourseRequestDto request)
    {
        course.Title = request.Title;
        course.Description = request.Description;
        course.ImageUrl = request.ImageUrl;
        course.Price = request.Price;
        course.Coin = request.Coin;
        course.CourseLanguageId = request.CourseLanguageId;
        course.UpdatedAt = DateTimeOffset.UtcNow;
    }
}