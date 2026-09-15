using CQRS.Core.Commands;
using CQRS.Core.Infostructure;

namespace Post.Cmd.Infostructure.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();
    public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
    {
        if (_handlers.ContainsKey(typeof(T)))
        {
            throw new IndexOutOfRangeException("Command handler already registered");
        }
        _handlers.Add(typeof(T), command => handler((T)command));
    }

    public async Task SendAsync(BaseCommand command)
    {
        if (_handlers.TryGetValue(command.GetType(), out var handler))
        {
            await handler(command);
        }
        else
        {
            throw new ArgumentNullException(nameof(command),  "Command handler not found");
        }
    }
    
}