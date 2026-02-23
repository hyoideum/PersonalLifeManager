namespace PersonalLifeManager.DTOs;

public class HabitStatisticsDto
{
    public int HabitId { get; set; }

    public DateOnly From { get; set; }
    public DateOnly To { get; set; }

    public int TotalDays { get; set; }
    public int CompletedDays { get; set; }

    public double CompletionRate { get; set; } 
}