namespace PersonalLifeManager.Exceptions;

public class HabitNotFoundException : AppException
{
    public HabitNotFoundException()
        : base("Habit not found.")
    {
    }
}