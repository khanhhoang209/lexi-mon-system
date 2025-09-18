using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;

namespace LexiMon.Repository.Context;

public class LexiMonDbContext : DbContext, ILexiMonDbContext
{
    public LexiMonDbContext(DbContextOptions<LexiMonDbContext> options) : base(options)
    {

    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LexiMonDbContext).Assembly);
    }
}