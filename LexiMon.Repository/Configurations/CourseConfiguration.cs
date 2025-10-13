using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Course");
        builder.ConfigureAuditableProperties();
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(c => c.Description)
            .HasMaxLength(1000)
            .IsRequired();
        builder.Property(c => c.ImageUrl)
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(c => c.Price)
            .HasPrecision(18, 2)
            .IsRequired(false);
        builder.Property(x => x.Coin)
            .HasPrecision(18, 2)
            .IsRequired(false);
        builder.HasOne(x => x.CourseLanguage)
            .WithMany(x => x.Courses)
            .HasForeignKey(x => x.CourseLanguageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}