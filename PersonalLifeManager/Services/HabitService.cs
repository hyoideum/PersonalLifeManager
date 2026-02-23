using AutoMapper;
using PersonalLifeManager.DTOs;
using PersonalLifeManager.Exceptions;
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
        
        return habit == null ? null : mapper.Map<HabitDto>(habit);
    }

    public async Task<HabitDto> CreateAsync(string userId, CreateHabitDto dto)
    {
        var habit = mapper.Map<Habit>(dto);
        habit.UserId = userId;

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
            throw new HabitNotFoundException();
        
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
            throw new HabitNotFoundException();

        await repository.RestoreAsync(habit);
    }
    
    public async Task SeedDefaultHabitsAsync(string userId)
    {
        var defaultHabits = new List<CreateHabitDto>
        {
            new() { Name = "Wake up early", Description = "Get up by 7am" },
            new() { Name = "Drink water", Description = "Drink 2L of water" },
            new() { Name = "Exercise", Description = "Do at least 30 min of exercise" },
            new() { Name = "Read", Description = "Read at least 20 pages" },
            new() { Name = "Steps", Description = "Make at least 10000 steps" },
            new() { Name = "No sugar", Description = "Avoid sugar today" },
            new() { Name = "Cook healthy meal", Description = "Prepare healthy food" },
            new() { Name = "No alcohol", Description = "Avoid alcohol today" },
            new() { Name = "Bedtime", Description = "Got to bed before 23:00" },
            new() { Name = "Hobi", Description = "Do what you like for at least 15 minutes"}
        };

        foreach (var habit in defaultHabits)
        {
            await CreateAsync(userId, habit);
        }
    }

    public async Task<int> CountActiveAsync(string userId)
    {
        return await repository.CountActiveAsync(userId);
    }
}