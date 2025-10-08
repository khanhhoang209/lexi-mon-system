using LexiMon.Repository.Domains;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Mappers;

public static class AchievementMapper
{
    public static AchievementResponseDto ToResponse(this Achievement achievement)
    {
        return new AchievementResponseDto()
        {
            Id = achievement.Id,
            Name = achievement.Name,
            Description = achievement.Description,
            TargetValue = achievement.TargetValue,
            RewardExps = achievement.RewardExps,
            RewardCoins = achievement.RewardCoins,
            IsRepeatable = achievement.IsRepeatable,
            CreatedAt = achievement.CreatedAt,
            IsActive = achievement.Status
        };
    }
    
    public static Achievement ToAchievement(this AchievementRequestDto achievementResponseDto)
    {
        return new Achievement()
        {
            Id = Guid.NewGuid(),
            Name = achievementResponseDto.Name,
            Description = achievementResponseDto.Description,
            TargetValue = achievementResponseDto.TargetValue,
            RewardExps = achievementResponseDto.RewardExps,
            RewardCoins = achievementResponseDto.RewardCoins,
            IsRepeatable = achievementResponseDto.IsRepeatable,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = true
        };
    }
    
    public static void UpdateAchievement(this Achievement achievement, AchievementRequestDto achievementRequestDto)
    {
        achievement.Name = achievementRequestDto.Name;
        achievement.Description = achievementRequestDto.Description;
        achievement.TargetValue = achievementRequestDto.TargetValue;
        achievement.RewardExps = achievementRequestDto.RewardExps;
        achievement.RewardCoins = achievementRequestDto.RewardCoins;
        achievement.IsRepeatable = achievementRequestDto.IsRepeatable;
        achievement.UpdatedAt = DateTimeOffset.UtcNow;
    }
}