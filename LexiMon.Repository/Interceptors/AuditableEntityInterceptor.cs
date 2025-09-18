using LexiMon.Repository.Common;
using LexiMon.Repository.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LexiMon.Repository.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly TimeProvider _timeProvider;

    public AuditableEntityInterceptor(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var now = TimeConverter.ToVietNamTime(_timeProvider.GetUtcNow());

        foreach (var entry in context.ChangeTracker.Entries<IBaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = default;
                entry.Entity.DeletedAt = default;
                entry.Entity.Status = true;
            }
            else if (entry.State == EntityState.Modified)
            {
                // Don't modify CreatedAt on updates
                entry.Property(e => e.CreatedAt).IsModified = false;

                // Update Updated only if properties other than audit fields changed
                if (entry.Properties.Any(p => p.IsModified && !new[] {
                                                nameof(IBaseAuditableEntity.UpdatedAt),
                                                nameof(IBaseAuditableEntity.DeletedAt),
                                                nameof(IBaseAuditableEntity.Status) }.Contains(p.Metadata.Name)))
                {
                    entry.Entity.UpdatedAt = now;
                }

                // Handle soft delete
                var status = entry.Property(nameof(IBaseAuditableEntity.Status));
                if (status.IsModified)
                {
                    if ((bool)status.CurrentValue!)
                    {
                        entry.Entity.DeletedAt = default;
                    }
                    else
                    {
                        entry.Entity.DeletedAt = now;
                    }
                }
            }
        }
    }
}