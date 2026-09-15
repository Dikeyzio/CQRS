using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infostructure.Handlers;

public class EventHandler : IEventHandler
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public EventHandler(IPostRepository postRepository,
        ICommentRepository commentRepository)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    public async Task On(PostCreatedEvent @event)
    {
        var post = new PostEntity()
        {
            PostId = @event.Id,
            Author = @event.Author,
            DatePosted =  @event.DatePosted,
            Message = @event.Message,
        };
        await _postRepository.CreateAsync(post);
    }

    public async Task On(PostLikeEvent @event)
    {
        var curr = await _postRepository.GetByIdAsync(@event.Id);
        if(curr == null) return;
        curr.Likes++;
        await _postRepository.UpdateAsync(curr);
    }

    public async Task On(PostRemovedEvent @event)
    {
        await _postRepository.DeleteAsync(@event.Id);
    }

    public async Task On(CommentAddedEvent @event)
    {
        var comment = new CommentEntity()
        {
            PostId = @event.Id,
            CommentId = @event.CommentId,
            CommentDate = @event.CommentDate,
            Comment = @event.Comment,
            Username = @event.Username,
            Edited = false
        };
        await _commentRepository.CreateAsync(comment);
    }

    public async Task On(CommentRemoveEvent @event)
    {
        await _commentRepository.DeleteAsync(@event.CommentId);
    }

    public async Task On(CommentUpdatedEvent @event)
    {
        var  comment = await _commentRepository.GetByIdAsync(@event.CommentId);
        comment.Comment = @event.Comment;
        await _commentRepository.UpdateAsync(comment);
    } 
    
    public async Task On(MessageUpdateEvent @event)
    {
        var curr = await _postRepository.GetByIdAsync(@event.Id);
        if(curr == null) return;    
        curr.Message = @event.Message;
        await _postRepository.UpdateAsync(curr);
    }
}