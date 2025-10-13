using LexiMon.Repository.Common;

namespace LexiMon.Repository.Domains;

public class CourseLanguage : BaseAuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}