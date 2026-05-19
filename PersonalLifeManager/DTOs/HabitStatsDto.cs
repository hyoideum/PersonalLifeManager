namespace PersonalLifeManager.DTOs;

public class HabitStatsDto
{
    public int HabitId { get; set; }
    public string HabitName { get; set; }
    public int CompletedCount { get; set; }
    public int TotalDays { get; set; }
    public double CompletionRate { get; set; }
    public int CurrentStreak { get; set; }
}