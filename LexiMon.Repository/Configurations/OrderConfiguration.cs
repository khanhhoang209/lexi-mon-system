using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.ToTable("Order");

        builder.Property(x => x.PurchaseCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.CoinCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.PaidAt)
            .IsRequired();

        builder.Property(x => x.PaymentStatus)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Item)
            .WithOne(x => x.Order)
            .HasForeignKey<Order>(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // builder.HasOne(x => x.Course)
        //     .WithOne(x => x.Order)
        //     .HasForeignKey<Order>(x => x.CourseId)
        //     .OnDelete(DeleteBehavior.Restrict);

        builder.ConfigureAuditableProperties();
    }
}