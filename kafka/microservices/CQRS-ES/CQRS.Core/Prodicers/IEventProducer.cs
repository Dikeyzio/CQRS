using CQRS.Core.Events;

namespace CQRS.Core.Prodicers;

public interface IEventProducer
{
    Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent;
}