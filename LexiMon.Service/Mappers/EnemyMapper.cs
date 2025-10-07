using LexiMon.Repository.Domains;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Mappers;

public static class EnemyMapper
{
    public static EnemyResponseDto ToEnemyResponse(this Enemy e)
    {
        return new EnemyResponseDto()
        {
            EnemyId = e.Id,
            Name = e.Name,
            ImageUrl = e.ImageUrl,
            AnimationAttackUrl = e.AnimationAttackUrl,
            AnimationMoveUrl = e.AnimationMoveUrl,
            HelmerUrl = e.HelmerUrl,
            ArmorUrl = e.ArmorUrl,
            BootUrl = e.BootUrl,
            WeaponUrl = e.WeaponUrl,
            Quantity = e.Quantity,
            Description = e.Description,
            EnemyLevelId = e.EnemyLevelId,
            EnemyLevelName = e.EnemyLevel?.Name ?? string.Empty,
            IsActive = e.Status
        };
    }
    public static Enemy ToEnemy(this EnemyRequestDto request)
    {
        return new Enemy()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ImageUrl = request.ImageUrl,
            AnimationAttackUrl = request.AnimationAttackUrl,
            AnimationMoveUrl = request.AnimationMoveUrl,
            HelmerUrl = request.HelmerUrl,
            ArmorUrl = request.ArmorUrl,
            BootUrl = request.BootUrl,
            WeaponUrl = request.WeaponUrl,
            Quantity = request.Quantity,
            Description = request.Description,
            EnemyLevelId = request.EnemyLevelId,
            CreatedAt = DateTime.UtcNow,
            Status = true,
        };
    }
    
    public static void UpdateEnemy(this Enemy enemy, EnemyRequestDto request)
    {
        enemy.Name = request.Name;
        enemy.ImageUrl = request.ImageUrl;
        enemy.AnimationAttackUrl = request.AnimationAttackUrl;
        enemy.AnimationMoveUrl = request.AnimationMoveUrl;
        enemy.HelmerUrl = request.HelmerUrl;
        enemy.ArmorUrl = request.ArmorUrl;
        enemy.BootUrl = request.BootUrl;
        enemy.WeaponUrl = request.WeaponUrl;
        enemy.Quantity = request.Quantity;
        enemy.Description = request.Description;
        enemy.EnemyLevelId = request.EnemyLevelId;
        enemy.UpdatedAt = DateTime.UtcNow;
    }
}