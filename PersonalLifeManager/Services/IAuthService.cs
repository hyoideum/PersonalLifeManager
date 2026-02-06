using PersonalLifeManager.DTOs;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Services;

public interface IAuthService
{
    Task<AppUser?> RegisterAsync(UserDto.RegisterDto dto);
    Task<string?> LoginAsync(UserDto.LoginDto dto);
}