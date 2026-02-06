using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalLifeManager.Models;
using PersonalLifeManager.Services;
using static PersonalLifeManager.DTOs.UserDto;

namespace PersonalLifeManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<AppUser> userManager, ITokenService tokenService) : ControllerBase
{
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto userDto)
    {
        var user = new AppUser
        {
            UserName = userDto.Username,
            Email = userDto.Email,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
        };

        var result = await userManager.CreateAsync(user, userDto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User created");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto userDto)
    {
        var user = await userManager.FindByNameAsync(userDto.Username);

        if (user == null)
            return Unauthorized();

        var validPassword = await userManager.CheckPasswordAsync(user, userDto.Password);

        if (!validPassword)
            return Unauthorized();
        
        var token = tokenService.CreateToken(user);

        return Ok(token);
    }
}