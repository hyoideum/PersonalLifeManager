namespace PersonalLifeManager.DTOs;

public class ConsistencyDto
{
    public string HabitName { get; set; } = string.Empty;

    public int CompletedDays { get; set; }

    public int TotalDays { get; set; }

    public double ConsistencyRate { get; set; }
}