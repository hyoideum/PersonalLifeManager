using PersonalLifeManager.DTOs;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Repositories;

public interface IHabitEntryRepository : IRepository<HabitEntry>
{
    Task<List<HabitEntry>> GetByUserAsync(string userId, DateOnly? from = null, DateOnly? to = null);
    Task<HabitEntry?> GetByIdAsync(int id, string userId);
    Task<bool> ExistsAsync(string userId, int habitId, DateOnly date);
    Task<HabitEntry?> GetByHabitAndDateAsync(string userId, int habitId, DateOnly date);
    Task<List<DailyHabitOverviewDto>> GetDailyOverviewAsync(string userId, DateOnly date);
    Task<List<DateOnly>> GetCompletedForHabitAsync(int habitId, string userId);
    Task<List<DateOnly>> GetCompletedDatesAsync(int habitId, string userId, DateOnly from, DateOnly to);
    Task<int> CountActiveHabitsAsync(string userId);
    Task<int> CountEntriesAsync(string userId, DateOnly from, DateOnly to);
    Task<List<CalendarHeatmapDto>> GetHeatmapAsync(string userId, DateOnly from, DateOnly to);
    Task<List<HabitStatsDto>> GetHabitStatsAsync(string userId, DateOnly from, DateOnly to);
    Task<int> CountCompletedForDayAsync(string userId, DateOnly date);
    Task<List<DateOnly>> GetCompletedDatesAsync(string userId);
}