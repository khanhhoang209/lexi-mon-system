using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class AmimationTypeConfiguration : IEntityTypeConfiguration<AnimationType>
{
    public void Configure(EntityTypeBuilder<AnimationType> builder)
    {
        builder.HasKey(x => x.Id);

        builder.ToTable("AnimationType");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.ConfigureAuditableProperties();
    }
}