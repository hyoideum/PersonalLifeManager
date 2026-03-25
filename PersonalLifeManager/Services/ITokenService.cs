using PersonalLifeManager.Models;

namespace PersonalLifeManager.Services;

public interface ITokenService
{
    string CreateToken(AppUser user);
    string CreateRefreshToken();
}
