using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Question");
        builder.ConfigureAuditableProperties();
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Content)
            .HasMaxLength(2000)
            .IsRequired();
        builder.Property(q => q.LessonId)
            .IsRequired(false);
        builder.Property(q => q.CustomLessonId)
            .IsRequired(false);

        builder.HasOne(q => q.Lesson)
            .WithMany(l => l.Questions)
            .HasForeignKey(q => q.LessonId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(q => q.CustomLesson)
            .WithMany(c => c.Questions)
            .HasForeignKey(q => q.CustomLessonId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }
}