namespace Post.Cmd.Api.Exceptions;

public class AggregateNotFoundException : Exception
{
    public AggregateNotFoundException(string msg) : base(msg)
    {
        
    }
}