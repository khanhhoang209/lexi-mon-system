using LexiMon.Repository.Domains;
using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Mappers;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LexiMon.Service.Implements;

public class CharacterService : ICharacterService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CharacterService> _logger;
    public CharacterService(IUnitOfWork unitOfWork, ILogger<CharacterService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<ServiceResponse> UpdateCharacterAsync(
        string userId, 
        Guid id, 
        CharacterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Character, Guid>();
        var character = await repo.Query()
            .Include(c=> c.User)
            .Where(c => c.Id == id && c.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
        if (character == null)
        {
            _logger.LogWarning("Character with id {CharacterId} not found for user {UserId}", id, userId);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Character not found"
            };
        }

        character.UpdateCharacter(request);
        await repo.UpdateAsync(character, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Character with id {CharacterId} updated for user {UserId}", id, userId);
        return new ServiceResponse()
        {
            Succeeded = true,
            Message = "Character updated successfully"
        };
    }

    public async Task<ResponseData<CharacterResponseDto>> GetCharacterByUserAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Character, Guid>();
        var character = await repo.Query()
                                                .Include(c => c.User)
                                                .Where(c => c.UserId == userId)
                                                .Select(c => c.ToCharacterResponse())
                                                .FirstOrDefaultAsync(cancellationToken);
        if (character == null)
        {
            _logger.LogWarning("Character not found for user {UserId}", userId);
            return new ResponseData<CharacterResponseDto>()
            {
                Succeeded = false,
                Message = "Character not found"
            };
        }

        _logger.LogInformation("Character found for user {UserId}", userId);
        return new()
        {
            Succeeded = true,
            Message = "Character retrieved successfully",
            Data = character
        };
    }
}