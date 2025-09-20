using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class AnimationConfiguration : IEntityTypeConfiguration<Animation>
{
    public void Configure(EntityTypeBuilder<Animation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);

        builder.HasOne(x => x.Item)
            .WithMany(x => x.Animations)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AnimationType)
            .WithMany(x => x.Animations)
            .HasForeignKey(x => x.AnimationTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureAuditableProperties();
    }
}