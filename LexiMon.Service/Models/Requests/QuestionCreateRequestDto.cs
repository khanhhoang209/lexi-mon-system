using System.ComponentModel.DataAnnotations;

namespace LexiMon.Service.Models.Requests;

public class QuestionCreateRequestDto
{
    [Required(ErrorMessage = "Vui lòng nhập nội dung câu hỏi!")]
    public string Content { get; set; } = null!;
    
    public string? LessonId { get; set; }
    public string? CustomLessonId { get; set; }

    public List<AnswerUpsertDto> Answers { get; set; } = new();
}

public class QuestionUpdateRequestDto
{
    [Required(ErrorMessage = "Vui lòng nhập nội dung câu hỏi!")]
    public string Content { get; set; } = null!;
    public bool IsStudied {get; set; }

    public List<AnswerUpsertDto> Answers { get; set; } = new();
}

public class AnswerUpsertDto
{
    [Required(ErrorMessage = "Vui lòng nhập nội dung câu trả lời!")]
    public string Content { get; set; } = null!;
    public bool IsCorrect { get; set; }
}