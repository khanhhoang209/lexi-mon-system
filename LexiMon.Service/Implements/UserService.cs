using LexiMon.Repository.Domains;
using LexiMon.Repository.Enum;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository, IUnitOfWork unitOfWork, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _tokenRepository = tokenRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
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

    public async Task<ServiceResponse> RegisterAsync(RegisterRequestDto requestBody, string role)
    {
        var user = await _userManager.FindByEmailAsync(requestBody.Email);
        if (user != null)
        {
            return new ServiceResponse()
            {
                Succeeded = false,
                Message = "Tài khoản đã tồn tại!",
            };
        }

        user = new ApplicationUser()
        {
            UserName = requestBody.Email,
            Email = requestBody.Email,
            FirstName = requestBody.FirstName,
            LastName = requestBody.LastName,
            Address = requestBody.Address,
            BirthDate = requestBody.BirthDate,
            Gender = requestBody.Gender ?? Gender.Other
        };

        var result = await _userManager.CreateAsync(user, requestBody.Password);
        if (!result.Succeeded)
        {
            return new ServiceResponse
            {
                Succeeded = false,
                Message = "Không thể tạo tài khoản. Vui lòng thử lại!",
            };
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
        if (!addToRoleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return new ServiceResponse
            {
                Succeeded = false,
                Message = "Không thể tạo tài khoản. Vui lòng thử lại!",
            };
        }

        var character = new Character()
        {
            UserId = user.Id,
            Name = user.FirstName ?? "Steve",
            Level = 1,
            Exp = 0,
        };
        await _unitOfWork.GetRepository<Character, Guid>().AddAsync(character);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {Email} registered successfully with role {Role}", requestBody.Email, role);
        return new ResponseData<string>()
        {
            Succeeded = true,
            Message = "Tạo tài khoản thành công!",
            Data = user.Id
        };
    }
}