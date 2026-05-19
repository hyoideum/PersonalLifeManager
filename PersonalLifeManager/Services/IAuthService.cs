using PersonalLifeManager.DTOs;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Services;

public interface IAuthService
{
    Task<(AppUser?, IEnumerable<string> Errors)> RegisterAsync(UserDto.RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(UserDto.LoginDto dto);
}