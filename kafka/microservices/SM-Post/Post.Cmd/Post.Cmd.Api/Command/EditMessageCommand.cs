using CQRS.Core.Commands;

namespace Post.Cmd.Api.Command;

public class EditMessageCommand : BaseCommand
{
    public string Message { get; set; } 
}