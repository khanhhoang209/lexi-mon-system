using System.Collections;
using LexiMon.Repository.Common;
using LexiMon.Repository.Context;
using LexiMon.Repository.Interfaces;

namespace LexiMon.Repository.Implements;

public class UnitOfWork : IUnitOfWork
{
    private readonly LexiMonDbContext _context;
    private readonly Hashtable _repos = new();

    public UnitOfWork(LexiMonDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<T, TId> GetRepository<T, TId>()
        where T : BaseEntity<TId>
        where TId : notnull
    {
        var typeName = typeof(T).Name;
        if (_repos.ContainsKey(typeName))
            return (IGenericRepository<T, TId>)_repos[typeName]!;

        var repoInstance = new GenericRepository<T, TId>(_context);
        _repos.Add(typeName, repoInstance);
        return repoInstance;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() =>
        _context.Dispose();
}