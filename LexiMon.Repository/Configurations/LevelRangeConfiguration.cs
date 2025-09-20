using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class LevelRangeConfiguration : IEntityTypeConfiguration<LevelRange>
{
    public void Configure(EntityTypeBuilder<LevelRange> builder)
    {
        builder.ToTable("LevelRange");
        builder.ConfigureAuditableProperties();

        builder.HasKey(lv => lv.Id);

        builder.Property(lv => lv.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(lv => lv.FromExp).IsRequired();
        builder.Property(lv => lv.ToExp).IsRequired();
    }
}