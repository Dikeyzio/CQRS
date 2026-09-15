using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infostructure.Config;

namespace Post.Cmd.Infostructure.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventsStore;

    public EventStoreRepository(IOptions<MongoDbConfig> config)
    {
        var mongoClient = new MongoClient(config.Value.ConnectionString);
        var mongoDb = mongoClient.GetDatabase(config.Value.DatabaseName);
        _eventsStore = mongoDb.GetCollection<EventModel>(config.Value.CollectionName);
    }
    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _eventsStore.Find(x => x.AggregateIdentifier == aggregateId).ToListAsync().ConfigureAwait(false);
    }

    public async Task SaveAsync(EventModel @event)
    {
        await _eventsStore.InsertOneAsync(@event).ConfigureAwait(false);
    }
}