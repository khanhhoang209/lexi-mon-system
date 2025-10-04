using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
{
    public void Configure(EntityTypeBuilder<LessonProgress> builder)
    {
        builder.ToTable("LessonProgress");
        builder.ConfigureAuditableProperties();
        builder.HasKey(lp => lp.Id);
        builder.Property(lp => lp.LessonId)
            .IsRequired(false);
        builder.Property(lp => lp.CustomLessonId)
            .IsRequired(false);
        builder.Property(lp => lp.StartDate).IsRequired(false);
        builder.Property(lp => lp.EndDate).IsRequired(false);
        builder.HasOne(lp => lp.User)
            .WithMany()
            .HasForeignKey(lp => lp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}