namespace PersonalLifeManager.DTOs;

public class GlobalStatisticsDto
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }

    public int TotalDays { get; set; }
    public int TotalHabits { get; set; }
    public int TotalCompletions { get; set; }

    public double AveragePerDay { get; set; }
    public double CompletionRate { get; set; }
}