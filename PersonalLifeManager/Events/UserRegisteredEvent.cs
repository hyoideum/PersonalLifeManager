namespace PersonalLifeManager.Events;

public class UserRegisteredEvent(string userId)
{
    public string UserId { get; } = userId;
}