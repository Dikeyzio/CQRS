using CQRS.Core.Infostructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Command;
using Post.Cmd.Api.Exceptions;
using Post.Cmd.Domain.Aggregates;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class DeletePostController : ControllerBase
{
    private readonly ILogger<DeletePostController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public DeletePostController(ILogger<DeletePostController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePostAsync(Guid id,DeletePostCommand command)
    {
        _logger.Log(LogLevel.Information, "Delitin post ");
        try
        {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status200OK, new BaseResponse()
            {
                Message = $"{command.Id} post  has been deleted"
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
        catch (AggregateNotFoundException exception)
        {
            _logger.Log(LogLevel.Warning, "Client passed incorrect id");
            return BadRequest(new BaseResponse()
            {
                Message = exception.Message
            });
        }
        catch (Exception exception)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to deletin post";
            _logger.Log(LogLevel.Error, exception, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
            {
                Message = SAFE_ERROR_MESSAGE,
            });
        }
    }
}