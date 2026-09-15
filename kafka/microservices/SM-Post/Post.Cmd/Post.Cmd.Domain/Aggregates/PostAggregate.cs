using CQRS.Core.Domain;
using CQRS.Core.Events;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string _author;
    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public bool Active
    {
        get => _active;
        set => _active = value;
    }

    public PostAggregate()
    {

    }

    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent() { Id = id, Author = author, Message = message });
    }

    public void Apply(PostCreatedEvent @event)
    {
        Id = @event.Id;
        _active = true;
        _author = @event.Author;
    }

    public void EditMessage(string message)
    {
        if (!_active)
        {
            throw new InvalidOperationException("Cannot edit message when not active");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new InvalidOperationException($"Cannot be null or empty {nameof(message)} ");
        }

        RaiseEvent(new MessageUpdateEvent()
        {
            Id = Id,
            Message = message
        });

    }

    public void Apply(MessageUpdateEvent @event)
    {
        Id = @event.Id;
    }

    public void LikePost()
    {
        if (!_active)
        {
            throw new InvalidOperationException("Cannot like post when not active");
        }

        RaiseEvent(new PostLikeEvent()
        {
            Id = Id,
        });
    }

    public void Apply(PostLikeEvent @event)
    {
        Id = @event.Id;
    }

    public void AddComment(string username, string comment)
    {
        if (!_active)
        {
            throw new InvalidOperationException("Cannot add comment when not active");
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidOperationException($"Cannot be null or empty {nameof(comment)} ");

        }

        RaiseEvent(new CommentAddedEvent()
        {
            Id = Id,
            CommentId = Guid.NewGuid(),
            Username = username,
            Comment = comment,
            CommentDate = DateTime.UtcNow
        });

    }

    public void Apply(CommentAddedEvent @event)
    {
        Id = @event.Id;
        _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Username, @event.Comment));
    }

    public void EditComment(string username, string comment, Guid commentId)
    {
        if (!_active)
        {
            throw new InvalidOperationException("Cannot edit comment when not active");
        }
        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException($"Cannot change comment entered by another user {comment} ");
        }

        RaiseEvent(new CommentUpdatedEvent()
        {
            Id = Id,
            CommentId = commentId,
            Username = username,
            EditDate = DateTime.UtcNow,
            Comment = comment
        });
    }

    public void Apply(CommentUpdatedEvent @event)
    {
        Id = @event.Id;
        _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("Cannot remove comment when not active");
        }

        if (!_comments[commentId].Item1.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException($"Cannot remove comment entered by another user {username} ");
        }

        RaiseEvent(new CommentRemoveEvent()
        {
            Id = Id,
            CommentId = commentId,
        });
    }

    public void Apply(CommentRemoveEvent @event)
    {
        Id = @event.Id;
        _comments.Remove(@event.CommentId);
    }

    public void DeletePost(string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("The post already deleted");
        }

        if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException($"Cannot delete post {username} ");
        }

        RaiseEvent(new PostRemovedEvent()
        {
            Id = Id,
        });
    }

    public void Apply(PostRemovedEvent @event)
    {
        Id = @event.Id;
        _active = false;
    }
}