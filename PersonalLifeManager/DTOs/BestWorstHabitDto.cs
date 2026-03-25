namespace PersonalLifeManager.DTOs;

public class BestWorstHabitDto
{
    public IList<HabitStatsDto>? BestHabits { get; set; }
    public IList<HabitStatsDto>? WorstHabits { get; set; }
}