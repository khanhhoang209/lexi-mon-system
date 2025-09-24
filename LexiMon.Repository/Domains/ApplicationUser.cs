
using LexiMon.Repository.Common;
using LexiMon.Repository.Enum;
using Microsoft.AspNetCore.Identity;

namespace LexiMon.Repository.Domains;

public class ApplicationUser : IdentityUser, IBaseAuditableEntity
{
    public string? FirstName { get; set; } = null;
    public string? LastName { get; set; } = null;
    public string? Address { get; set; } = null;
    public DateTimeOffset? BirthDate { get; set; }
    public decimal Coins { get; set; } = 0;
    public Gender Gender { get; set; } = Gender.Other;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset DeletedAt { get; set; }
    public bool Status { get; set; }
    public Character? Character { get; set; } = null;
    public ICollection<UserDeck> UserDecks { get; set; } = new List<UserDeck>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<AchievementUser> Achievements { get; set; } = new List<AchievementUser>();
}