using System.Runtime.InteropServices.JavaScript;
using PersonalLifeManager.DTOs;

namespace PersonalLifeManager.Services;

public class DashboardService (IHabitService habitService, IHabitEntryService habitEntryService) : IDashboardService
{
    public async Task<DashboardDto> GetDashboardAsync(string userId, DateOnly fromDate, DateOnly toDate)
    {
        var to = toDate ==  DateOnly.MinValue ? DateOnly.FromDateTime(DateTime.UtcNow) : toDate;
        var from = fromDate == DateOnly.MinValue ? to.AddDays(-30) : fromDate;
        
        var globalStats =
            await habitEntryService.GetGlobalStatisticsAsync(userId, from, to);

        var (best, worst) =
            await habitEntryService.GetBestAndWorstHabitAsync(userId, from, to);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var todayCompleted =
            await habitEntryService.CountCompletedForDayAsync(userId, today);

        var totalHabits =
            await habitService.CountActiveAsync(userId);

        var currentStreak =
            await habitEntryService.GetCurrentStreakAsync(userId, today);

        return new DashboardDto
        {
            GlobalStatistics = globalStats,
            BestHabits = best,
            WorstHabits = worst,
            TodayCompleted = todayCompleted,
            TotalHabits = totalHabits,
            CurrentStreak = currentStreak
        };
    }
}