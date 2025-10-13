using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class CourseLanguageConfiguration : IEntityTypeConfiguration<CourseLanguage>
{
    public void Configure(EntityTypeBuilder<CourseLanguage> builder)
    {
        builder.ToTable("CourseLanguage");

        builder.HasKey(x => x.Id);

        builder.ConfigureAuditableProperties();
    }
}