using LexiMon.Repository.Domains;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Mappers;

public static class CharacterMapper
{
    public static CharacterResponseDto ToCharacterResponse(this Character character)
    {
        return new()
        {
            Id = character.Id,
            Name = character.Name,
            Level = character.Level,
            Exp = character.Exp,
            HelmetUrl = character.HelmetUrl,
            ArmorUrl = character.ArmorUrl,
            WeaponUrl = character.WeaponUrl,
            BootUrl = character.BootUrl,
            UserId = character.UserId,
        };
    }

    public static void UpdateCharacter(this Character character, CharacterRequestDto request)
    {
        character.Name = request.Name;
        character.Level = request.Level;
        character.Exp = request.Exp;
        character.HelmetUrl = request.HelmetUrl;
        character.ArmorUrl = request.ArmorUrl;
        character.WeaponUrl = request.WeaponUrl;
        character.BootUrl = request.BootUrl;
        character.UpdatedAt = DateTimeOffset.UtcNow;
    }
}