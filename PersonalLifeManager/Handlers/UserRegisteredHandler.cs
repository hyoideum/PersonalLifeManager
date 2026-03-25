using PersonalLifeManager.Events;
using PersonalLifeManager.Services;

namespace PersonalLifeManager.Handlers;

public class UserRegisteredHandler(IHabitService habitService) : IEventHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent @event)
    {
        await habitService.SeedDefaultHabitsAsync(@event.UserId);
    }
}