using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface IUserDeckService
{

    Task<PaginatedResponse<UserDeckResponseDto>> GetUserDecksByUserIdAsync(
        string userId,
        GetUserDeckRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ResponseData<UserDeckResponseDto>> GetUserDeckByIdAsync(
        Guid userDeckId,
        CancellationToken cancellationToken = default);
    
    
}