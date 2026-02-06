using PersonalLifeManager.Models;

namespace PersonalLifeManager.Repositories;

public interface IHabitRepository : IRepository<Habit>
{
    Task<Habit?> GetByIdAsync(int id, string userId);
    Task<List<Habit>> GetAllAsync(string userId);
    Task<List<Habit>> GetAllAsyncIncludeDeleted(string userId);
}