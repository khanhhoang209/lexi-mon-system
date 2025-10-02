namespace LexiMon.Service.Models.Responses;

//Response by questionId
public class QuestionResponseDto
{
    public Guid QuestionId { get; set; }
    public string Content { get; set; } = null!;
    
    public Guid? LessonId { get; set; }
    public string? LessonTitle { get; set; } = null!;
    public Guid? CustomLessonId { get; set; }
    public string? CustomLessonTitle { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool IsStudied {get; set; }
    public List<AnswerResponseDto> Answers { get; set; } = new();
}


//Response with CustomLessonId
public class QuestionCustomLessonResponseDto
{
    public Guid QuestionId { get; set; }
    public string Content { get; set; } = null!;
    public Guid? CustomLessonId { get; set; }
    public string? CustomLessonTitle { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool IsStudied {get; set; }
    public List<AnswerResponseDto> Answers { get; set; } = new();
}

//Response with LessonId
public class QuestionLessonResponseDto
{public Guid QuestionId { get; set; }
    public string Content { get; set; } = null!;
    public Guid? LessonId { get; set; }
    public string? LessonTitle { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool IsStudied {get; set; }
    public List<AnswerResponseDto> Answers { get; set; } = new();
}


public class AnswerResponseDto
{
    public Guid? AnswerId { get; set; } 
    public string Content { get; set; } = null!;
    public bool IsCorrect { get; set; }
}
