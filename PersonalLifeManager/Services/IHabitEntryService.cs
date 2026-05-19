using PersonalLifeManager.DTOs;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Services;

public interface IHabitEntryService
{
    Task<HabitEntryDto> AddEntryAsync(CreateHabitEntryDto dto, string userId);
    Task<List<HabitEntryDto>> GetEntriesAsync(string userId, DateOnly from, DateOnly to);
    Task<HabitEntryDto?> GetByIdAsync(int id, string userId);
    Task DeleteEntryAsync(int id, string userId);
    Task<HabitEntryDto> ToggleAsync(int habitId, DateOnly date, string userId);
    Task<List<DailyHabitOverviewDto>> GetDailyOverviewAsync(string userId, DateOnly date);
    Task<HabitStreakDto> GetStreakAsync(int habitId, string userId);
    Task<HabitStatisticsDto> GetStatisticsAsync(int habitId, string userId, DateOnly? from, DateOnly? to);
    Task<List<HabitStatisticsDto>> GetStatisticsForAllHabitsAsync(string userId, DateOnly? from, DateOnly? to, DateOnly? day);
    Task<GlobalStatisticsDto> GetGlobalStatisticsAsync(string userId, DateOnly from, DateOnly to);
    Task<List<CalendarHeatmapDto>> GetHeatmapAsync(string userId, DateOnly from, DateOnly to);
    Task<(List<HabitStatsDto>? Best, List<HabitStatsDto>? Worst)> GetBestAndWorstHabitAsync(string userId, DateOnly from, DateOnly to);
    Task<int> CountCompletedForDayAsync(string userId, DateOnly date);
    Task<int> GetCurrentStreakAsync(string userId, DateOnly today);
    Task<int> GetLongestStreakAsync(string userId);
    Task<WeeklyAnalyticsDto> GetWeeklyAnalyticsAsync(string userId);
    Task<ConsistencyDto?> GetMostConsistentHabitAsync(string userId);
    // Task<List<HabitStatsDto>> GetHabitStatsAdvancedAsync(string userId, DateOnly from, DateOnly to);
}