using Microsoft.EntityFrameworkCore;
using PersonalLifeManager.Data;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Repositories;

public class HabitRepository(AppDbContext context) : Repository<Habit>(context), IHabitRepository
{
    public async Task<Habit?> GetByIdAsync(int id, string userId)
    {
        return await DbSet.FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);
    }

    public async Task<List<Habit>> GetAllAsync(string userId)
    {
        return await DbSet.Where(h => h.UserId == userId).ToListAsync();
    }

    public async Task<List<Habit>> GetAllAsyncIncludeDeleted(string userId)
    {
        return await context.Habits.Where(h => h.UserId == userId).IgnoreQueryFilters().ToListAsync();
    }
}