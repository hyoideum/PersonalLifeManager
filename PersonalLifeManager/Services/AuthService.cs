using PersonalLifeManager.DTOs;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Services;

public class AuthService : IAuthService
{
    public Task<AppUser?> RegisterAsync(UserDto.RegisterDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<string?> LoginAsync(UserDto.LoginDto dto)
    {
        throw new NotImplementedException();
    }
}