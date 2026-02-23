namespace PersonalLifeManager.DTOs;

public class HabitEntryDto
{
    public int Id { get; set; }
    public int HabitId { get; set; }
    public string Name { get; set; }
    public DateOnly Date { get; set; }
    public string UserId { get; set; }
    public string? Note  { get; set; }
}