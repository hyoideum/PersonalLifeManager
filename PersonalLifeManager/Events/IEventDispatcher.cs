namespace PersonalLifeManager.Events;

public interface IEventDispatcher
{
    Task Dispatch<T>(T @event);
}