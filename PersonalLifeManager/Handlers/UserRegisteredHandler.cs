using PersonalLifeManager.Events;
using PersonalLifeManager.Services;

namespace PersonalLifeManager.Handlers;

public class UserRegisteredHandler(IHabitService habitService) : IEventHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent @event)
    {
        Console.WriteLine("HANDLER STEP 1");
        
        await habitService.SeedDefaultHabitsAsync(@event.UserId);
        
        Console.WriteLine("HANDLER STEP 2");
    }
}