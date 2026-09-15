using CQRS.Core.Infostructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Command;
using Post.Cmd.Api.Exceptions;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class EditMessageController : ControllerBase
{
    private readonly ILogger<EditMessageController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;
    public EditMessageController(
        ILogger<EditMessageController> logger,
        ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher =  commandDispatcher;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditMessageAsync(Guid id,EditMessageCommand editMessageCommand)
    {
        try
        {
            editMessageCommand.Id = id;
            _logger.LogInformation($"Editing message {editMessageCommand.Id}");
            await _commandDispatcher.SendAsync(editMessageCommand);
            return Ok(new BaseResponse()
            {
                Message = "Success",
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
            const string SAFE_ERROR_MESSAGE = "Error while processing request to edit message";
            _logger.Log(LogLevel.Error, exception, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
            {
                Message = SAFE_ERROR_MESSAGE,
            });
        }
       
    }
}