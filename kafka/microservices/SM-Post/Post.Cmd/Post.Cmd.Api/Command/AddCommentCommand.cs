using CQRS.Core.Commands;

namespace Post.Cmd.Api.Command;

public class AddCommentCommand : BaseCommand
{
    public string Comment { get; set; }
    public string Username { get; set; }
}