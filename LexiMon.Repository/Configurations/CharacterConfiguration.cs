using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.HasKey(x => x.Id);

        builder.ToTable("Character");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Level)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(x => x.Exp)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(x => x.User)
            .WithOne(x => x.Character)
            .HasForeignKey<Character>(x => x.UserId);

        builder.ConfigureAuditableProperties();
    }
}