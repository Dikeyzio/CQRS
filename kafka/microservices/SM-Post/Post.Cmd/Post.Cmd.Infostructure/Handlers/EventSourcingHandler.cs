using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infostructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infostructure.Handlers;

public class EventSourcingHandler: IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore _eventStore;

    public EventSourcingHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }
    public async Task<PostAggregate> GetByIdAsync(Guid id)
    {
        var aggregate = new PostAggregate();
        var events =  await _eventStore.GetEventsAsync(id);
        if (events == null || !events.Any())
        {
            return aggregate;
        }
        aggregate.ReplayEvents(events);
        var latestVersion = events.Select(e => e.Version).Max();
        aggregate.Version = latestVersion;
        return aggregate;
    }

    public async Task SaveAsync(AggregateRoot aggregateRoot)
    {
       await _eventStore.SaveEventAsync(aggregateRoot.Id, aggregateRoot.GetUncommitedChanges(), aggregateRoot.Version);
       aggregateRoot.MarkChangesAsCommitted();
    }
}