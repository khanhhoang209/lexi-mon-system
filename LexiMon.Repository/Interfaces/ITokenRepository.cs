using LexiMon.Repository.Domains;

namespace LexiMon.Repository.Interfaces;

public interface ITokenRepository
{
    (string, int) GenerateJwtToken(ApplicationUser user, string role);
}