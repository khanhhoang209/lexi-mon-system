using System.ComponentModel.DataAnnotations;

namespace LexiMon.Service.Models.Requests;

public class LessonRequestDto
{    
    [Required(ErrorMessage = "Vui lòng nhập tiêu đề!")]
    [MaxLength(300, ErrorMessage = "Tiêu đề không được vượt quá 300 ký tự!")]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public Guid CourseId { get; set; }
}

