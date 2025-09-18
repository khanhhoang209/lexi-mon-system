using LexiMon.Repository.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public static class AuditableEntityConfigurationExtensions
{
    public static void ConfigureAuditableProperties<T>(this EntityTypeBuilder<T> builder) where T : class
    {
        builder.Property(nameof(IBaseAuditableEntity.CreatedAt))
            .IsRequired();

        builder.Property(nameof(IBaseAuditableEntity.UpdatedAt))
            .IsRequired();

        builder.Property(nameof(IBaseAuditableEntity.DeletedAt))
            .IsRequired();

        builder.Property(nameof(IBaseAuditableEntity.Status))
            .IsRequired();
    }
}