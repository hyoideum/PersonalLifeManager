namespace PersonalLifeManager.DTOs;

public class HabitStatisticsDto
{
    public int HabitId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; } = "";
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public int TotalDays { get; set; }
    public int CompletedDays { get; set; }
    public double CompletionRate { get; set; } 
    public bool CompletedToday { get; set; }
}