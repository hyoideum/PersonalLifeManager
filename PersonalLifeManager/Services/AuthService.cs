using Microsoft.AspNetCore.Identity;
using PersonalLifeManager.Data;
using PersonalLifeManager.DTOs;
using PersonalLifeManager.Events;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Services;

public class AuthService(UserManager<AppUser> userManager, IRefreshTokenService refreshTokenService, ITokenService tokenService,
    IEventDispatcher eventDispatcher) : IAuthService
{
    // public async Task<(AppUser?, IEnumerable<string> Errors)> RegisterAsync(UserDto.RegisterDto dto)
    // {
    //     var user = new AppUser
    //     {
    //         UserName = dto.Username,
    //         Email = dto.Email,
    //         FirstName = dto.FirstName,
    //         LastName = dto.LastName,
    //     };
    //
    //     var result = await userManager.CreateAsync(user, dto.Password);
    //
    //     if (!result.Succeeded)
    //     {
    //         var errors = result.Errors.Select(e => e.Description);
    //         return (null, errors);
    //     }
    //     
    //     // await eventDispatcher.Dispatch(new UserRegisteredEvent(user.Id));
    //
    //     return (user, Enumerable.Empty<string>());
    // }
    
    public async Task<(AppUser?, IEnumerable<string> Errors)> RegisterAsync(UserDto.RegisterDto dto)
    {
        Console.WriteLine("REGISTER STEP 1");

        var user = new AppUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
        };
        
        try
        {
            Console.WriteLine("BEFORE CREATE");
            
            var result = await userManager.CreateAsync(user, dto.Password);
            
            Console.WriteLine("AFTER CREATE");
        } 
        catch (Exception ex)
        {
            Console.WriteLine("CREATE EXCEPTION");
            Console.WriteLine(ex.ToString());
        }


        // if (!result.Succeeded)
        // {
        //     Console.WriteLine("REGISTER FAILED");
        //
        //     foreach (var error in result.Errors)
        //     {
        //         Console.WriteLine(error.Description);
        //     }
        //
        //     return (null, result.Errors.Select(e => e.Description));
        // }

        Console.WriteLine("REGISTER STEP 4");

        // await eventDispatcher.Dispatch(new UserRegisteredEvent(user.Id));

        Console.WriteLine("REGISTER STEP 5");

        return (user, Enumerable.Empty<string>());
    }

    // public async Task<AuthResponseDto> LoginAsync(UserDto.LoginDto userDto)
    // {
    //     var user = await userManager.FindByNameAsync(userDto.Username);
    //
    //     if (user == null || !await userManager.CheckPasswordAsync(user, userDto.Password))
    //         throw new UnauthorizedAccessException("Invalid credentials");
    //     
    //     var token = tokenService.CreateToken(user);
    //     var refreshToken = await refreshTokenService.CreateAsync(user.Id);
    //     
    //     return new AuthResponseDto
    //     {
    //         AccessToken = token,
    //         RefreshToken = refreshToken
    //     };
    // }
    
    public async Task<AuthResponseDto> LoginAsync(UserDto.LoginDto userDto)
    {
        Console.WriteLine("LOGIN STEP 1");

        var user = await userManager.FindByNameAsync(userDto.Username);

        Console.WriteLine("LOGIN STEP 2");

        if (user == null)
        {
            Console.WriteLine("USER NOT FOUND");
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var passwordValid =
            await userManager.CheckPasswordAsync(user, userDto.Password);

        Console.WriteLine("LOGIN STEP 3");

        if (!passwordValid)
        {
            Console.WriteLine("INVALID PASSWORD");
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        Console.WriteLine("LOGIN STEP 4");

        var token = tokenService.CreateToken(user);

        Console.WriteLine("LOGIN STEP 5");

        var refreshToken =
            await refreshTokenService.CreateAsync(user.Id);

        Console.WriteLine("LOGIN STEP 6");

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