namespace PersonalLifeManager.DTOs;

public class DashboardDto
{
    public GlobalStatisticsDto GlobalStatistics { get; set; }
    public WeeklyAnalyticsDto WeeklyAnalytics { get; set; }

    public IList<HabitStatsDto>? BestHabits { get; set; }
    public IList<HabitStatsDto>? WorstHabits { get; set; }
    public ConsistencyDto? MostConsistentHabit { get; set; }
    
    public IList<CalendarHeatmapDto> Heatmap { get; set; }

    public int TodayCompleted { get; set; }
    public int TotalHabits { get; set; }

    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
}