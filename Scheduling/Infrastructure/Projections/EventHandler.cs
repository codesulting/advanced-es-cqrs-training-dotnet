using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scheduling.EventSourcing;

namespace Scheduling.Infrastructure.Projections;

public class EventHandler
{
    private ulong? lastProcessedEventPosition;

    readonly List<EventHandlerEnvelope> _handlers = new();

    protected void When<T>(Func<T, EventMetadata, Task> when)
    {
        _handlers.Add(new EventHandlerEnvelope(typeof(T), (e, m) => when((T)e, m)));
    }

    public async Task Handle(Type eventType, object e, EventMetadata m)
    {
        var handlers = _handlers
            .Where(h => h.EventType == eventType)
            .ToList();

        foreach (var handler in handlers)
        {
            await CheckIdempotency(m.Position, () => handler.Handler(e, m));
        }
    }

    private Task CheckIdempotency(ulong? position, Func<Task> repoTask)
    {
        if (lastProcessedEventPosition == null || position.GetValueOrDefault() - 1 == lastProcessedEventPosition)
        {
            lastProcessedEventPosition = position;
            return repoTask();
        }

        return Task.CompletedTask;
    }

    public bool CanHandle(Type eventType)
    {
        return _handlers.Any(h => h.EventType == eventType);
    }

    public record EventHandlerEnvelope(Type EventType,
        Func<object, EventMetadata, Task> Handler);
}