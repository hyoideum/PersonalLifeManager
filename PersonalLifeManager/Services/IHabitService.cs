using PersonalLifeManager.DTOs;

namespace PersonalLifeManager.Services;

public interface IHabitService
{
    Task<List<HabitDto>> GetAllAsync(string userId);
    Task<HabitDto?> GetByIdAsync(int id, string userId);
    Task<HabitDto> CreateAsync(string userId, CreateHabitDto habitDto);
    Task<HabitDto?> UpdateAsync(int id, UpdateHabitDto dto, string userId);
    Task DeleteAsync(int id, string userId);
    Task<List<HabitDto>> GetAllIncludingDeletedAsync(string userId);
    Task RestoreAsync(int id, string userId);
}