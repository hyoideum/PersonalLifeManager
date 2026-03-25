using PersonalLifeManager.Handlers;

namespace PersonalLifeManager.Events;

public class EventDispatcher(IServiceProvider serviceProvider) : IEventDispatcher
{
    public async Task Dispatch<T>(T @event)
    {
        var handlerType = typeof(IEnumerable<IEventHandler<T>>);
        var handlers = serviceProvider.GetServices<IEventHandler<T>>();
        
        foreach (var handler in handlers)
        {
            var method = handler?.GetType().GetMethod("Handle");
            if (method == null) continue;
            var task = (Task)method.Invoke(handler, new object[] { @event });
            if (task != null) await task;
        }

    }
}