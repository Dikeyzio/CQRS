using CQRS.Core.Infostructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Command;
using Post.Cmd.Api.Exceptions;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class EditCommentController : ControllerBase
{
    private readonly ILogger<EditCommentController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;
    public EditCommentController(
        ILogger<EditCommentController> logger,
        ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher =  commandDispatcher;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditMessageAsync(Guid id,EditCommentCommand editCommentCommand)
    {
        try
        {
            editCommentCommand.Id = id;
            _logger.LogInformation($"Editing comment {editCommentCommand.Id}");
            await _commandDispatcher.SendAsync(editCommentCommand);
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
            const string SAFE_ERROR_MESSAGE = "Error while processing request to edit comment";
            _logger.Log(LogLevel.Error, exception, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
            {
                Message = SAFE_ERROR_MESSAGE,
            });
        }
       
    }
}