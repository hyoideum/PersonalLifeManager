using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PersonalLifeManager.Controllers;

public class BaseApiController : ControllerBase
{
    protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) 
                               ?? throw new UnauthorizedAccessException("User not authenticated");
}