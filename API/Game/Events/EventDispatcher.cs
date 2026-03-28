namespace API.Game.Events;

public class EventDispatcher
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);

        if (!_handlers.TryGetValue(type, out var list))
        {
            list = new List<Delegate>();
            _handlers[type] = list;
        }

        list.Add(handler);
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);

        if (_handlers.TryGetValue(type, out var list))
        {
            list.Remove(handler);
        }
    }

    public void Publish<T>(T @event)
    {
        var type = typeof(T);

        if (_handlers.TryGetValue(type, out var list))
        {
            // copy to avoid modification during iteration
            foreach (var handler in list.ToArray())
            {
                ((Action<T>)handler)(@event);
            }
        }
    }
}