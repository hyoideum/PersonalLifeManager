namespace PersonalLifeManager.DTOs;

public class DashboardDto
{
    public GlobalStatisticsDto GlobalStatistics { get; set; }

    public HabitStatsDto? BestHabit { get; set; }
    public HabitStatsDto? WorstHabit { get; set; }

    public int TodayCompleted { get; set; }
    public int TotalHabits { get; set; }

    public int CurrentStreak { get; set; }
}