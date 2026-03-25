using Microsoft.AspNetCore.Identity;
using PersonalLifeManager.DTOs;
using PersonalLifeManager.Events;
using PersonalLifeManager.Models;
using PersonalLifeManager.Repositories;

namespace PersonalLifeManager.Services;

public class AuthService(UserManager<AppUser> userManager, IHabitService habitService, IRefreshTokenService refreshTokenService, ITokenService tokenService,
    IEventDispatcher eventDispatcher) : IAuthService
{
    public async Task<AppUser?> RegisterAsync(UserDto.RegisterDto dto)
    {
        var user = new AppUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new Exception(result.Errors.First().Description);
        
        await eventDispatcher.Dispatch(new UserRegisteredEvent(user.Id));

        return user;
    }

    public async Task<AuthResponseDto> LoginAsync(UserDto.LoginDto userDto)
    {
        var user = await userManager.FindByNameAsync(userDto.Username);

        if (user == null || !await userManager.CheckPasswordAsync(user, userDto.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        // var validPassword = await userManager.CheckPasswordAsync(user, userDto.Password);
        //
        // if (!validPassword)
        //     throw new UnauthorizedAccessException("Invalid credentials");
        
        var token = tokenService.CreateToken(user);
        var refreshToken = await refreshTokenService.CreateAsync(user.Id);
        
        return new AuthResponseDto
        {
            AccessToken = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshRequest req)
    {
        var userId = await refreshTokenService.ValidateAndGetUserIdAsync(req.RefreshToken);
        
        var user = await userManager.FindByIdAsync(userId);
            
        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        var newAccessToken = tokenService.CreateToken(user);
        var newRefreshToken = await refreshTokenService.RotateAsync(userId, req.RefreshToken);

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(RefreshRequest req)
    {
        await refreshTokenService.RevokeAsync(req.RefreshToken);
    }
}