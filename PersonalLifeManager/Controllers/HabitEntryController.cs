using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalLifeManager.DTOs;
using PersonalLifeManager.Services;

namespace PersonalLifeManager.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HabitEntryController(IHabitEntryService service) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateHabitEntry(CreateHabitEntryDto dto)
    {
        var habitEntryDto = await service.AddEntryAsync(dto, UserId);
        return Ok(habitEntryDto);
    }
    
    [HttpGet]
    public async Task<ActionResult<List<HabitEntryDto>>> GetAll(DateOnly startDate, DateOnly endDate)
    {
        return await service.GetEntriesAsync(UserId, startDate, endDate);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<HabitEntryDto>> GetById(int id)
    {
        return Ok(await service.GetByIdAsync(id, UserId));
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteEntryAsync(id, UserId);
        
        return Ok();
    }
    
    [HttpPost("toggle")]
    public async Task<ActionResult<HabitEntryDto>> ToggleHabit([FromQuery] int habitId, [FromQuery] DateOnly date)
    {
        return Ok(await service.ToggleAsync(habitId, date, UserId));
    }

    [HttpGet("daily")]
    public async Task<ActionResult<List<DailyHabitOverviewDto>>> GetDailyHabitOverview(DateOnly date)
    {
        return Ok(await service.GetDailyOverviewAsync(UserId, date));
    }

    [HttpGet("streak")]
    public async Task<ActionResult<HabitStreakDto>> GetStreak(int habitId)
    {
        return Ok(await service.GetStreakAsync(habitId, UserId));
    }
    
    [HttpGet("{habitId}/statistics")]
    public async Task<ActionResult<HabitStatisticsDto>> GetStatistics(int habitId, [FromQuery] DateOnly from,
        [FromQuery] DateOnly to)
    {
        return Ok(await service.GetStatisticsAsync(habitId, UserId, from, to));
    }
    
    [HttpGet("statistics/global")]
    public async Task<ActionResult<GlobalStatisticsDto>> GetGlobalStatistics([FromQuery] DateOnly from,
        [FromQuery] DateOnly to)
    {
        return Ok(await service.GetGlobalStatisticsAsync(UserId, from, to));
    }
    
    [HttpGet("statistics/heatmap")]
    public async Task<ActionResult<List<CalendarHeatmapDto>>> GetHeatmap([FromQuery] DateOnly from,
        [FromQuery] DateOnly to)
    {
        return Ok(await service.GetHeatmapAsync(UserId, from, to));
    }
    
    [HttpGet("statistics/best-worst")]
    public async Task<ActionResult<BestWorstHabitDto>> GetBestWorstHabit([FromQuery] DateOnly from,
        [FromQuery] DateOnly to)
    {
        var (best, worst) =
            await service.GetBestAndWorstHabitAsync(UserId, from, to);

        return Ok(new BestWorstHabitDto
        {
            BestHabit = best,
            WorstHabit = worst
        });
    }
}