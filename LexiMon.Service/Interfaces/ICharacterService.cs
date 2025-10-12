using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface ICharacterService
{
    
    Task<ServiceResponse> UpdateCharacterAsync(
        string userId,
        Guid id,
        CharacterRequestDto request,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<CharacterResponseDto>> GetCharacterByUserAsync(
        string userId,
        CancellationToken cancellationToken = default);
    
}