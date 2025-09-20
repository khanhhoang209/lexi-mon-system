using LexiMon.Repository.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexiMon.Repository.Configurations;

public class UserDeckConfiguration : IEntityTypeConfiguration<UserDeck>
{
    public void Configure(EntityTypeBuilder<UserDeck> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserDecks)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // builder.HasOne(x => x.Course)
        //     .WithMany(x => x.UserDecks)
        //     .HasForeignKey(x => x.CourseId)
        //     .OnDelete(DeleteBehavior.Cascade);

    }
}