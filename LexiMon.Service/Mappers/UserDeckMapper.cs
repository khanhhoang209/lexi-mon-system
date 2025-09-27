using LexiMon.Repository.Domains;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Mappers;

public static class UserDeckMapper
{
    public static UserDeckResponseDto ToUserDeckResponse(this UserDeck deck)
    {
        return new UserDeckResponseDto()
        {
            UserDeckId = deck.Id,
            CourseId = deck.CourseId,
            CourseTitle = deck.Course != null ? deck.Course.Title : null,
            CourseDescription = deck.Course != null ? deck.Course.Description : null,
            CourseImageUrl = deck.Course != null ? deck.Course.ImageUrl : null,
            CustomLessonId = deck.CustomLessonId,
            CustomLessonTitle = deck.CustomLesson != null ? deck.CustomLesson.Title : null,
            CustomLessonDescription = deck.CustomLesson != null ? deck.CustomLesson.Description : null,
            IsActive = deck.Status   
        };
    }
}