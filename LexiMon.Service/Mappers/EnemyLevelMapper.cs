using LexiMon.Repository.Domains;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Mappers;

public static class EnemyLevelMapper
{
    public static EnemyLevelResponseDto ToEnemyLevelResponse(this EnemyLevel e)
    {
        return new EnemyLevelResponseDto()
        {
            Id = e.Id,
            Name = e.Name,
            FromLevel = e.FromLevel,
            ToLevel = e.ToLevel,
            IsActive = e.Status
        };
    }
    
    public static EnemyLevel ToEnemyLevel(this EnemyLevelRequestDto request)
    {
        return new EnemyLevel()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            FromLevel = request.FromLevel,
            ToLevel = request.ToLevel,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = true,
        };
    }
    
    public static void UpdateEnemyLevel(this EnemyLevel e, EnemyLevelRequestDto request)
    {
        e.Name = request.Name;
        e.FromLevel = request.FromLevel;
        e.ToLevel = request.ToLevel;
        e.UpdatedAt = DateTimeOffset.UtcNow;
    }
}