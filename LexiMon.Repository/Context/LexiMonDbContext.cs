using LexiMon.Repository.Domains;
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
    }
}