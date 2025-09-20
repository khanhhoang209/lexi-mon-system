using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.Ignore(x => x.Id);

        builder.HasKey(x => new { x.CharacterId, x.ItemId });

        builder.ToTable("Equipment");

        builder.HasOne(x => x.Character)
            .WithMany(c => c.Equipments)
            .HasForeignKey(x => x.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Item)
            .WithMany(i => i.Equipments)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureAuditableProperties();
    }
}