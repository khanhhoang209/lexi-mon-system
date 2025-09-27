using LexiMon.Service.ApiResponse;
using LexiMon.Service.Implements;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface IQuestionService
{
    Task<ResponseData<List<Guid>>> CreateQuestionAnswerAsync(
        List<QuestionCreateRequestDto> requests,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse> UpdateQuestionReplaceAsync(
        Guid id,
        QuestionUpdateRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> DeleteQuestionAsync(
        Guid id, CancellationToken cancellationToken = default);
    
    Task<ResponseData<QuestionResponseDto>> GetQuestionByIdAsync(
        Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<QuestionLessonResponseDto>> GetQuestionsByLessonIdAsync(
        Guid lessonId, 
        GetQuestionRequest request,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedResponse<QuestionCustomLessonResponseDto>> GetQuestionsByCustomLessonIdAsync(
        Guid customLessonId, 
        GetQuestionRequest request,
        CancellationToken cancellationToken = default);
    

}