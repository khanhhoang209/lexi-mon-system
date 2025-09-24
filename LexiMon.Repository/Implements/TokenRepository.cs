using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LexiMon.Repository.Domains;
using LexiMon.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LexiMon.Repository.Implements;

public class TokenRepository : ITokenRepository
{
    private readonly IConfiguration _configuration;

    public TokenRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string, int) GenerateJwtToken(ApplicationUser user, string role)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.UserName!),
            new Claim(ClaimTypes.Role, role)
        };
        var expiration = double.Parse(_configuration["JwtSettings:AccessTokenExpirationSeconds"]
            ?? throw new ArgumentException("Expiration time cannot be null"));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]
            ?? throw new ArgumentException("Key cannot be null")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["JwtSettings:Issuer"],
            _configuration["JwtSettings:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiration),
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), (int)expiration);
    }
}