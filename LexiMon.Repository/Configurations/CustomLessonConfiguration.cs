using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class CustomLessonConfiguration : IEntityTypeConfiguration<CustomLesson>
{
    public void Configure(EntityTypeBuilder<CustomLesson> builder)
    {
        builder.ToTable("CustomLesson");
        
        builder.ConfigureAuditableProperties();
        
        builder.HasKey(cl => cl.Id);
        
        builder.Property(cl => cl.Description)
            .HasMaxLength(1000)
            .IsRequired(false);
        
        builder.Property(cl => cl.Title)
            .HasMaxLength(300);
        
        builder.HasOne(cl => cl.LessonProgress)
            .WithOne(lp => lp.CustomLesson)
            .HasForeignKey<LessonProgress>(lp => lp.CustomLessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}