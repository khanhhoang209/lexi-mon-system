using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class EnemyConfiguration : IEntityTypeConfiguration<Enemy>
{
    public void Configure(EntityTypeBuilder<Enemy> builder)
    {
        builder.ToTable("Enemy");
        builder.ConfigureAuditableProperties();

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(e => e.AnimationAttackUrl)
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(e => e.AnimationMoveUrl)
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(e => e.HelmerUrl)
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(e => e.ArmorUrl)
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(e => e.BootUrl)
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(e => e.WeaponUrl)
            .HasMaxLength(500)
            .IsRequired(false);
        
        builder.HasOne(e => e.EnemyLevel)
            .WithMany(el => el.Enemies)
            .IsRequired()
            .HasForeignKey(e => e.EnemyLevelId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}