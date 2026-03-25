namespace PersonalLifeManager.DTOs;

public class DashboardDto
{
    public GlobalStatisticsDto GlobalStatistics { get; set; }

    public IList<HabitStatsDto>? BestHabits { get; set; }
    public IList<HabitStatsDto>? WorstHabits { get; set; }

    public int TodayCompleted { get; set; }
    public int TotalHabits { get; set; }

    public int CurrentStreak { get; set; }
}