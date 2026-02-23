using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalLifeManager.DTOs;
using PersonalLifeManager.Services;

namespace PersonalLifeManager.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController(IDashboardService service) : BaseApiController
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardDto>> GetDashboard(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to)
    {
        return Ok(await service.GetDashboardAsync(UserId, from, to));
    }
}