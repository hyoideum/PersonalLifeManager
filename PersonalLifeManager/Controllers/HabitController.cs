using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalLifeManager.DTOs;
using PersonalLifeManager.Services;

namespace PersonalLifeManager.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HabitController(IHabitService service) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateHabit(CreateHabitDto dto)
    {
        var habitDto = await service.CreateAsync(UserId, dto);

        return Ok(habitDto); 
    }
    
    [HttpGet]
    public async Task<ActionResult<List<HabitDto>>> GetAll()
    {
        return await service.GetAllAsync(UserId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HabitDto>> GetById(int id)
    {
        return Ok(await service.GetByIdAsync(id, UserId));
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHabit(int id, UpdateHabitDto dto)
    {
        var habitDto = await service.UpdateAsync(id, dto, UserId);

        if (habitDto == null)
            return NotFound();

        return Ok(habitDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id, UserId);
        
        return Ok();
    }
    
    [HttpGet("all")]
    public async Task<ActionResult<List<HabitDto>>> GetAllIncludeDeleted(string userId)
    {
        return await service.GetAllIncludingDeletedAsync(userId);
    }
    
    [HttpPut("restore/{id}")]
    public async Task<IActionResult> Restore(int id)
    {
        await service.RestoreAsync(id, UserId);
        
        return Ok();
    }
}