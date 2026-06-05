using PersonalLifeManager.Handlers;

namespace PersonalLifeManager.Events;

public class EventDispatcher(IServiceProvider serviceProvider) : IEventDispatcher
{
    // public async Task Dispatch<T>(T @event)
    // {
    //     var handlerType = typeof(IEnumerable<IEventHandler<T>>);
    //     var handlers = serviceProvider.GetServices<IEventHandler<T>>();
    //     
    //     foreach (var handler in handlers)
    //     {
    //         var method = handler?.GetType().GetMethod("Handle");
    //         if (method == null) continue;
    //         var task = (Task)method.Invoke(handler, new object[] { @event });
    //         if (task != null) await task;
    //     }
    //
    // }
    
    public async Task Dispatch<T>(T @event)
    {
        Console.WriteLine("DISPATCH START");

        try
        {
            var handlers = serviceProvider.GetServices<IEventHandler<T>>();

            Console.WriteLine($"HANDLERS COUNT: {handlers.Count()}");

            foreach (var handler in handlers)
            {
                Console.WriteLine($"RUNNING {handler.GetType().Name}");

                await handler.Handle(@event);

                Console.WriteLine($"FINISHED {handler.GetType().Name}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("EVENT ERROR");
            Console.WriteLine(ex.ToString());
            throw;
        }
        
        Console.WriteLine("DISPATCH END");
    }
}