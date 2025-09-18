using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;

namespace LexiMon.Repository.Context;

public interface ILexiMonDbContext
{
    DbSet<Product> Products { get; }
}