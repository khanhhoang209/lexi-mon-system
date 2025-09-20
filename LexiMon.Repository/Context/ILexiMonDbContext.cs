using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;

namespace LexiMon.Repository.Context;

public interface ILexiMonDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Achievement> Achievements { get; }
    DbSet<AchievementUser> AchievementUsers { get; }
    DbSet<Animation> Animations { get; }
    DbSet<AnimationType> AnimationTypes { get; }
    DbSet<Answer> Answers { get; }
    DbSet<Category> Categories { get; }
    DbSet<Character> Characters { get; }
    DbSet<Course> Courses { get; }
    DbSet<CustomLesson> CustomLessons { get; }
    DbSet<Enemy> Enemies { get; }
    DbSet<EnemyLevel> EnemyLevels { get; }
    DbSet<Equipment> Equipments { get; }
    DbSet<Item> Items { get; }
    DbSet<Lesson> Lessons { get; }
    DbSet<LessonProgress> LessonProgresses { get; }
    DbSet<LevelRange> LevelRanges { get; }
    DbSet<Order> Orders { get; }
    DbSet<Question> Questions { get; }
    DbSet<UserDeck> UserDecks { get; }
}