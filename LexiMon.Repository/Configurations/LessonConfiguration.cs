using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lesson");
        builder.ConfigureAuditableProperties();
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Description)
            .IsRequired(false)
            .HasMaxLength(1000);
        
        builder.Property(l => l.Title)
            .HasMaxLength(300);
        
        builder.HasOne(l => l.Course)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.CourseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}