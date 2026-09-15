using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Prodicers;
using Microsoft.Extensions.Options;

namespace Post.Cmd.Api.Produsers;

public class EventProducer : IEventProducer
{
    private readonly ProducerConfig _prodcerConfig;

    public EventProducer(IOptions<ProducerConfig> prodcerConfig)
    {
        _prodcerConfig = prodcerConfig.Value;
    }
    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        using var producer = new ProducerBuilder<string, string>(_prodcerConfig)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();
        var eventMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event, @event.GetType())
        };
        var deliveryResult = await producer.ProduceAsync(topic, eventMessage);
        if (deliveryResult.Status == PersistenceStatus.NotPersisted)
        {
            throw new Exception($"Could not produce {@event.GetType().Name} message to topic {topic} due to following reason {deliveryResult.Message}");
        }
    }
}