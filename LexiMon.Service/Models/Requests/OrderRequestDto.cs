using LexiMon.Repository.Enum;

namespace LexiMon.Service.Models.Requests;

public class OrderRequestDto
{
    public Guid? CourseId { get; set; }
    public Guid? ItemId { get; set; }
}