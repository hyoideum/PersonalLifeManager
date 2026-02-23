using PersonalLifeManager.Models;

namespace PersonalLifeManager.DTOs;

public class CreateHabitEntryDto
{
    public int HabitId { get; set; }
    public DateOnly Date { get; set; }
    public string? Note { get; set; }
}