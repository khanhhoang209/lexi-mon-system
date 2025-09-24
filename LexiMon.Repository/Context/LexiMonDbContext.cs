using LexiMon.Repository.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LexiMon.Repository.Context;

public class LexiMonDbContext : IdentityDbContext<ApplicationUser>, ILexiMonDbContext
{
    public LexiMonDbContext(DbContextOptions<LexiMonDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<AchievementUser> AchievementUsers => Set<AchievementUser>();
    public DbSet<Animation> Animations => Set<Animation>();
    public DbSet<AnimationType> AnimationTypes => Set<AnimationType>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CustomLesson> CustomLessons => Set<CustomLesson>();
    public DbSet<Enemy> Enemies => Set<Enemy>();
    public DbSet<EnemyLevel> EnemyLevels => Set<EnemyLevel>();
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();
    public DbSet<LevelRange> LevelRanges => Set<LevelRange>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<UserDeck> UserDecks => Set<UserDeck>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LexiMonDbContext).Assembly);

        // Seed roles
        modelBuilder.Entity<IdentityRole>().HasData(SeedingRoles());

        // Seed users
        modelBuilder.Entity<ApplicationUser>().HasData(SeedingUsers());

        // Seed user roles
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(SeedingUserRoles());
    }

    private ICollection<IdentityRole> SeedingRoles()
    {
        return new List<IdentityRole>()
        {
            new IdentityRole()
            {
                Id = "3631e38b-60dd-4d1a-af7f-a26f21c2ef82",
                Name = "Free",
                NormalizedName = "FREE",
                ConcurrencyStamp = "3631e38b-60dd-4d1a-af7f-a26f21c2ef82"
            },
            new IdentityRole()
            {
                Id = "51ef7e08-ff07-459b-8c55-c7ebac505103",
                Name = "Premium",
                NormalizedName = "PREMIUM",
                ConcurrencyStamp = "51ef7e08-ff07-459b-8c55-c7ebac505103"
            },
            new IdentityRole()
            {
                Id = "37a7c5df-4898-4fd4-8e5f-d2abd4b57520",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "37a7c5df-4898-4fd4-8e5f-d2abd4b57520"
            }
        };
    }

    private ICollection<ApplicationUser> SeedingUsers()
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        return new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = "5d7efb6d-0d52-4159-ab2e-7fd356973925",
                UserName = "free@example.com",
                NormalizedUserName = "FREE@EXAMPLE.COM",
                Email = "free@example.com",
                NormalizedEmail = "FREE@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null!, "12345aA@"),
                SecurityStamp = Guid.NewGuid().ToString(),
                Status = true
            },
            new ApplicationUser
            {
                Id = "c2765f80-383f-46f2-9a73-ec47863100ae",
                UserName = "premium@example.com",
                NormalizedUserName = "PREMIUM@EXAMPLE.COM",
                Email = "premium@example.com",
                NormalizedEmail = "PREMIUM@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null!, "12345aA@"),
                SecurityStamp = Guid.NewGuid().ToString(),
                Status = true
            },
            new ApplicationUser
            {
                Id = "88f1581b-4f4e-4831-8cf8-ee4afed04c11",
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null!, "12345aA@"),
                SecurityStamp = Guid.NewGuid().ToString(),
                Status = true
            }
        };
    }

    private ICollection<IdentityUserRole<string>> SeedingUserRoles()
    {
        return new List<IdentityUserRole<string>>
        {
            new IdentityUserRole<string>
                { UserId = "5d7efb6d-0d52-4159-ab2e-7fd356973925", RoleId = "3631e38b-60dd-4d1a-af7f-a26f21c2ef82" }, // free
            new IdentityUserRole<string>
                { UserId = "c2765f80-383f-46f2-9a73-ec47863100ae", RoleId = "51ef7e08-ff07-459b-8c55-c7ebac505103" }, // premium
            new IdentityUserRole<string>
                { UserId = "88f1581b-4f4e-4831-8cf8-ee4afed04c11", RoleId = "37a7c5df-4898-4fd4-8e5f-d2abd4b57520" } // admin
        };
    }
}