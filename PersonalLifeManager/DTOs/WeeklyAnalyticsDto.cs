namespace PersonalLifeManager.DTOs;

public class WeeklyAnalyticsDto
{
    public double CompletionRate { get; set; }
    public double PreviousWeekCompletionRate { get; set; }
    public double Trend { get; set; }
    public string BestDay { get; set; } = string.Empty;
    public int CompletedHabits { get; set; }
    public int TotalHabits { get; set; }
}