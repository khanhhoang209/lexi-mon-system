using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class EnemyLevelConfiguration : IEntityTypeConfiguration<EnemyLevel>
{
    public void Configure(EntityTypeBuilder<EnemyLevel> builder)
    {
        builder.ToTable("EnemyLevel");
        builder.ConfigureAuditableProperties();

        builder.HasKey(el => el.Id);

        builder.Property(el => el.Name)
            .HasMaxLength(100)
            .IsRequired();
    }
}