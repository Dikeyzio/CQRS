using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Infostructure;
using CQRS.Core.Prodicers;
using Post.Cmd.Api.Exceptions;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Stores;

public class EventStore : IEventStore
{
    private readonly IEventStoreRepository _eventsStoreRepository;
    private readonly IEventProducer _eventsProducer;

    public EventStore(
        IEventStoreRepository eventsStore,
        IEventProducer eventProducer)
    {
        _eventsStoreRepository =  eventsStore;
        _eventsProducer = eventProducer;
    }

    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await _eventsStoreRepository.FindByAggregateId(aggregateId);
        if (eventStream == null || !eventStream.Any())
        {
            throw new AggregateNotFoundException("Incorrect post ID provided");
        }
        return eventStream.OrderBy(x => x.Version).Select(x =>x.EventData).ToList();
    }

    public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await _eventsStoreRepository.FindByAggregateId(aggregateId);
        if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
        {
            throw new ConcurrencyException();
        }

        var version = expectedVersion;
        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            var eventType = @event.GetType().Name;
            var eventModel = new EventModel()
            {
                Timestamp = DateTime.UtcNow,
                AggregateIdentifier = aggregateId,
                AggregateType = nameof(PostAggregate),
                Version = version,
                EventType = eventType,
                EventData = @event
            };
            await _eventsStoreRepository.SaveAsync(eventModel);
            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            await _eventsProducer.ProduceAsync(topic, @event);
        }
        
    }
}