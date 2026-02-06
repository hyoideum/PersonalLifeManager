using AutoMapper;
using PersonalLifeManager.DTOs;
using PersonalLifeManager.Models;
using PersonalLifeManager.Repositories;

namespace PersonalLifeManager.Services;

public class HabitService(IHabitRepository repository, IMapper mapper) : IHabitService
{
    public async Task<List<HabitDto>> GetAllAsync(string userId)
    {
        var habits = await repository.GetAllAsync(userId);
        
        return mapper.Map<List<HabitDto>>(habits).OrderBy(h => h.Id).ToList();
    }

    public async Task<HabitDto?> GetByIdAsync(int id, string userId)
    {
        var habit = await repository.GetByIdAsync(id, userId);
        
        if (habit == null)
            return null;
        
        return mapper.Map<HabitDto>(habit);
    }

    public async Task<HabitDto> CreateAsync(string userId, CreateHabitDto dto)
    {
        var habit = new Habit
        {
            Name = dto.Name,
            Description = dto.Description,
            UserId = userId
        };

        await repository.AddAsync(habit);
        await repository.SaveChangesAsync();

        return mapper.Map<HabitDto>(habit);
    }

    public async Task<HabitDto?> UpdateAsync(int id, UpdateHabitDto dto, string userId)
    {
        var habit = await repository.GetByIdAsync(id, userId);

        if (habit == null || habit.IsDeleted)
            return null;

        habit.Name = dto.Name;
        habit.Description = dto.Description;

        await repository.SaveChangesAsync();

        return mapper.Map<HabitDto>(habit);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var habit = await repository.GetByIdAsync(id, userId);
        
        if (habit == null)
            throw new KeyNotFoundException($"Habit not found for this user!");
        
        await repository.SoftDeleteAsync(habit);
    }

    public async Task<List<HabitDto>> GetAllIncludingDeletedAsync(string userId)
    {
        var habits = await repository.GetAllAsyncIncludeDeleted(userId);
        
        return mapper.Map<List<HabitDto>>(habits).OrderBy(h => h.Id).ToList();
    }

    public async Task RestoreAsync(int id, string userId)
    {
        var habit = await repository
            .GetAllIncludingDeletedAsync() 
            .ContinueWith(t => t.Result.FirstOrDefault(h => h.Id == id && h.UserId == userId));

        if (habit == null)
            throw new KeyNotFoundException("Habit not found for this user.");

        await repository.RestoreAsync(habit);
    }
}