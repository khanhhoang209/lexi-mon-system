using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class AchievementUserConfiguration : IEntityTypeConfiguration<AchievementUser>
{
    public void Configure(EntityTypeBuilder<AchievementUser> builder)
    {
        builder.ToTable("AchievementUser");

        builder.Ignore(x => x.Id);

        builder.HasKey(x => new { x.UserId, x.AchievementId });

        builder.HasOne(x => x.User)
            .WithMany(x => x.Achievements)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Achievement)
            .WithMany(x => x.AchievementUsers)
            .HasForeignKey(x => x.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}