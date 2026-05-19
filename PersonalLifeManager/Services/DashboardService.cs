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

        var weeklyStats = await habitEntryService.GetWeeklyAnalyticsAsync(userId);

        var (best, worst) =
            await habitEntryService.GetBestAndWorstHabitAsync(userId, from, to);
        
        var mostConsistentHabit = await habitEntryService.GetMostConsistentHabitAsync(userId);

        var heatmap = await habitEntryService.GetHeatmapAsync(userId, from.AddDays(-179), to);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var todayCompleted =
            await habitEntryService.CountCompletedForDayAsync(userId, today);

        var totalHabits =
            await habitService.CountActiveAsync(userId);

        var currentStreak =
            await habitEntryService.GetCurrentStreakAsync(userId, today);
        
        var longestStreak = await habitEntryService.GetLongestStreakAsync(userId);
        
        return new DashboardDto
        {
            GlobalStatistics = globalStats,
            WeeklyAnalytics = weeklyStats,
            BestHabits = best,
            WorstHabits = worst,
            MostConsistentHabit =  mostConsistentHabit,
            Heatmap = heatmap,
            TodayCompleted = todayCompleted,
            TotalHabits = totalHabits,
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak
        };
    }
}