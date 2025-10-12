using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;

namespace LexiMon.Service.Interfaces;

public interface IUserService
{
    Task<ServiceResponse> LoginAsync(LoginRequestDto requestBody);
    Task<ServiceResponse> RegisterAsync(RegisterRequestDto requestBody, string role);
    Task<ServiceResponse> GetUserByIdAsync(CancellationToken cancellationToken = default);
    Task<ServiceResponse> UpdateAsync(UserRequestDto requestBody, CancellationToken cancellationToken = default);
    Task<ServiceResponse> UpdateResourceAsync(UserResourseDto requestBody, CancellationToken cancellationToken = default);
}