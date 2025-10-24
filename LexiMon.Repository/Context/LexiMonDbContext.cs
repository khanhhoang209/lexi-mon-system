using LexiMon.Repository.Constants;
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
    public DbSet<CourseLanguage> CourseLanguages => Set<CourseLanguage>();

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

        // Seed characters
        modelBuilder.Entity<Character>().HasData(SeedingCharacters());

        // Seed categories
        modelBuilder.Entity<Category>().HasData(SeedingCategories());

        // Seed items
        modelBuilder.Entity<Item>().HasData(SeedingItems());

        // Seed course languages
        modelBuilder.Entity<CourseLanguage>().HasData(SeedingLanguages());
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

    private ICollection<Character> SeedingCharacters()
    {
        return new List<Character>
        {
            new Character
            {
                Id = Guid.Parse("bfc27875-fba8-4e0b-a6d0-e5caa591abf5"),
                UserId = "5d7efb6d-0d52-4159-ab2e-7fd356973925",
                Name = "User Free Character",
                Level = 1,
                Exp = 0,
                Status = true
            },
            new Character
            {
                Id = Guid.Parse("c661f028-9f2a-4c10-a7c7-503d977c4092"),
                UserId = "c2765f80-383f-46f2-9a73-ec47863100ae",
                Name = "User Premium Character",
                Level = 1,
                Exp = 0,
                Status = true
            },
        };
    }

    private ICollection<Category> SeedingCategories()
    {
        return new List<Category>
        {
            new Category()
            {
                Id = Guid.Parse("a6368fa8-5017-4de2-a15c-719546923a1d"),
                Name = Constants.Categories.PremiumPackage,
                Status = true
            }
        };
    }

    private ICollection<Item> SeedingItems()
    {
        return new List<Item>
        {
            new Item()
            {
                Id = Guid.Parse("083e0832-47a5-449a-9c60-5f61db49140d"),
                CategoryId = Guid.Parse("a6368fa8-5017-4de2-a15c-719546923a1d"),
                Name = "Premium Package",
                Description = "Unlock the full Leximon experience with the Premium Package. Enjoy exclusive features, unlimited access to all content, faster progression, and special rewards that enhance both gameplay and customization. Designed for dedicated users who want the ultimate experience.",
                Price = 49000,
                ImageUrl = "https://leximon.blob.core.windows.net/images/premium-package.png",
                IsPremium = false,
                Status = true,
            },
            new Item()
            {
                Id = Guid.Parse("96423DE3-75E5-4337-B3D0-83F1FBDB90A5"),
                CategoryId = Guid.Parse("a6368fa8-5017-4de2-a15c-719546923a1d"),
                Name = "Starter Package",
                Description = "Get started with the basic features of Leximon. The Starter Package lets you explore essential tools, play freely, and experience the system without any cost. Perfect for new users who want to learn and progress step by step before unlocking premium advantages.",
                ImageUrl = "https://leximon.blob.core.windows.net/images/starter-package.png",
                Price = 0,
                IsPremium = false,
                Status = true,
            }
        };
    }

    private ICollection<CourseLanguage> SeedingLanguages()
    {
        return new List<CourseLanguage>
        {
            new CourseLanguage()
            {
                Id = Guid.Parse("fc101062-1f47-4c71-8825-267148542026"),
                Name = Languages.English,
                Status = true
            },
            new CourseLanguage()
            {
                Id = Guid.Parse("a45ac5e8-25c9-4c1d-9ba4-7d0b49d5601b"),
                Name = Languages.Japanese,
                Status = true
            },
            new CourseLanguage()
            {
                Id = Guid.Parse("0dcb03a7-5885-4611-a170-bd5ca18e9076"),
                Name = Languages.Korean,
                Status = true
            },
            new CourseLanguage()
            {
                Id = Guid.Parse("5c98c202-bfce-418b-8820-d3727793659f"),
                Name = Languages.Chinese,
                Status = true
            }
        };
    }
}