using LexiMon.Repository.Domains;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Mappers;

public static class LevelRangeMapper
{
    public static LevelRangeResponseDto ToResponse(this LevelRange l)
    {
        return new LevelRangeResponseDto
        {
            Id = l.Id,
            Name = l.Name,
            FromExp = l.FromExp,
            ToExp = l.ToExp,
            IsActive = l.Status,
            CreatedAt = l.CreatedAt
        };
    }
    
    public static LevelRange ToLevelRange(this LevelRangeRequestDto request)
    {
        return new LevelRange
        {
            Name = request.Name,
            FromExp = request.FromExp,
            ToExp = request.ToExp,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
    
    public static void UpdateLevelRange(this LevelRange levelRange, LevelRangeRequestDto request)
    {
        levelRange.Name = request.Name;
        levelRange.FromExp = request.FromExp;
        levelRange.ToExp = request.ToExp;
        levelRange.UpdatedAt = DateTimeOffset.UtcNow;
    }
}