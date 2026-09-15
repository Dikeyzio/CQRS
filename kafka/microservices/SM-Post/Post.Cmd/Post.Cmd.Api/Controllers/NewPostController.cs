using CQRS.Core.Infostructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Command;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class NewPostController : ControllerBase
{
    private  readonly ILogger<NewPostController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public NewPostController(ILogger<NewPostController> logger,
        ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task<ActionResult> NewPostAsync(NewPostCommand command)
    {
        try
        {
            var id = Guid.NewGuid();
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new NewPostResponse()
            {
                Message = $"{id} has been created"
            });
        }
        catch (InvalidOperationException exception)
        {
            _logger.Log(LogLevel.Warning, "Client made a bad request");
            return BadRequest(new BaseResponse()
            {
                Message = exception.Message
            });
        }
        catch (Exception exception)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to create new post";
            _logger.Log(LogLevel.Error, exception, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse()
            {
                Message = SAFE_ERROR_MESSAGE,
                Id = command.Id
            });
        }
}
    
}