using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answer");
        builder.ConfigureAuditableProperties();
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.IsCorrect)
            .IsRequired();

        builder.HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}