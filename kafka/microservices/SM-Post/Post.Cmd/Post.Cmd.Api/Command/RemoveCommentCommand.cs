using CQRS.Core.Commands;

namespace Post.Cmd.Api.Command;

public class RemoveCommentCommand : BaseCommand
{
    public Guid CommentId { get; set; }
    public string Username { get; set; }
}