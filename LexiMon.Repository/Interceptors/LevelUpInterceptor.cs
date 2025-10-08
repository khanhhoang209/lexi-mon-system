using LexiMon.Repository.Domains;
using LexiMon.Repository.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LexiMon.Repository.Interceptors;

public class LevelUpInterceptor : SaveChangesInterceptor
{

    public LevelUpInterceptor()
    {
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var ctx = eventData.Context;
        if (ctx == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var entries = ctx.ChangeTracker.Entries<Character>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .Where(e => e.Property(x => x.Exp).IsModified || e.State == EntityState.Added)
            .ToList();

        if (entries.Count == 0) return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var ranges = await ctx.Set<LevelRange>().OrderBy(r => r.FromExp).ToListAsync(cancellationToken);

        foreach (var entry in entries)
        {
            var ch = entry.Entity;
            var oldLevel = ch.Level;
            var newLevel = LevelCalculator.CalculateLevel(ch.Exp, ranges);
            if (newLevel != oldLevel)
            {
                ch.Level = newLevel;
                entry.Property(x => x.Level).IsModified = true;
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}