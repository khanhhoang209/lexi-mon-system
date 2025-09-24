using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;

namespace LexiMon.Service.Interfaces;

public interface IUserService
{
    Task<ServiceResponse> LoginAsync(LoginRequestDto requestBody);
    Task<ServiceResponse> RegisterAsync(RegisterRequestDto requestBody, string role);
}