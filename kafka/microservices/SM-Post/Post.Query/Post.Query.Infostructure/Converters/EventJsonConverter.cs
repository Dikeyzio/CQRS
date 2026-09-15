using System.Text.Json;
using System.Text.Json.Serialization;
using CQRS.Core.Events;
using Post.Common.Events;

namespace Post.Query.Infostructure.Converters;

public class EventJsonConverter: JsonConverter<BaseEvent>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return  typeToConvert == typeof(BaseEvent);
    }

    public override BaseEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out var document))
        {
            throw new JsonException($"Cannot convert {typeToConvert} to {nameof(BaseEvent)}.");
        }

        if (!document.RootElement.TryGetProperty("Type", out var typeElement)) 
        {
            throw new JsonException($"Cannot convert {typeToConvert} to {nameof(BaseEvent)}.");
        }
        var type = typeElement.GetString();
        var json = document.RootElement.GetRawText();
        return type switch
        {
            nameof(PostCreatedEvent) => JsonSerializer.Deserialize<PostCreatedEvent>(json, options),
            nameof(CommentAddedEvent) => JsonSerializer.Deserialize<CommentAddedEvent>(json, options),
            nameof(CommentRemoveEvent) => JsonSerializer.Deserialize<CommentRemoveEvent>(json, options),
            nameof(CommentUpdatedEvent) => JsonSerializer.Deserialize<CommentUpdatedEvent>(json, options),
            nameof(MessageUpdateEvent) => JsonSerializer.Deserialize<MessageUpdateEvent>(json, options),
            nameof(PostLikeEvent) => JsonSerializer.Deserialize<PostLikeEvent>(json, options),
            nameof(PostRemovedEvent) => JsonSerializer.Deserialize<PostRemovedEvent>(json, options),
            _ => throw new JsonException($"{type} is not a valid event type.")
        };
    }

    public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
    {
        
    }
}