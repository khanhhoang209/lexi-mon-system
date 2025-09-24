using LexiMon.Repository.Domains;
using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LexiMon.Service.Implements;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenRepository _tokenRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _tokenRepository = tokenRepository;
        _logger = logger;
    }

    public async Task<ServiceResponse> LoginAsync(LoginRequestDto requestBody)
    {
        var user = await _userManager.FindByEmailAsync(requestBody.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, requestBody.Password))
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", requestBody.Email);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Sai email hoặc mật khẩu!",
            };
        }

        if (!user.Status)
        {
            _logger.LogWarning("Disabled account login attempt for email: {Email}", requestBody.Email);
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Tài khoản đã bị vô hiệu hóa!",
            };
        }

        var role = (await _userManager.GetRolesAsync(user))[0];
        var (token, expire) = _tokenRepository.GenerateJwtToken(user, role);

        var response = new LoginResponseDto()
        {
            Token = token,
            ExpiredIn = expire,
        };

        _logger.LogInformation("User {Email} logged in successfully with role {Role}", requestBody.Email, role);
        return new ResponseData<LoginResponseDto>()
        {
            Succeeded = true,
            Message = "Đăng nhập thành công!",
            Data = response
        };
    }
}