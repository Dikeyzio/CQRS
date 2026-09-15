using Post.Common.Events;

namespace Post.Query.Infostructure.Handlers;

public interface IEventHandler
{
    Task On(CommentAddedEvent @event);
    Task On(CommentRemoveEvent @event);
    Task On(CommentUpdatedEvent @event);
    Task On(MessageUpdateEvent @event);
    Task On(PostCreatedEvent @event);
    Task On(PostLikeEvent @event);
    Task On(PostRemovedEvent @event);
}