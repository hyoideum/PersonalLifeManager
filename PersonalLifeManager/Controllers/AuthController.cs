using Microsoft.AspNetCore.Mvc;
using PersonalLifeManager.DTOs;
using PersonalLifeManager.Services;
using static PersonalLifeManager.DTOs.UserDto;

namespace PersonalLifeManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto userDto)
    {
        return Ok(await authService.RegisterAsync(userDto));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto userDto)
    {
        try
        {
            var response = await authService.LoginAsync(userDto);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
    {
        try
        {
            var response = await authService.RefreshTokenAsync(req);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest req)
    {
        await authService.LogoutAsync(req);
        return Ok();
    }
}