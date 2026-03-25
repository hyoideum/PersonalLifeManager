namespace PersonalLifeManager.Handlers;

public interface IEventHandler<T>
{
    Task Handle(T @event);
}