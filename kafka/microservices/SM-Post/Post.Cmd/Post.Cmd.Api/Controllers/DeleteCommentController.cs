using CQRS.Core.Infostructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Command;
using Post.Cmd.Api.Exceptions;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class DeleteCommentController : ControllerBase
{
    private readonly ILogger<DeleteCommentController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public DeleteCommentController(ILogger<DeleteCommentController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCommentAsync(Guid id,RemoveCommentCommand removeCommentCommand)
    {
        try
        {
            _logger.Log(LogLevel.Information, "Deleting comment");
            removeCommentCommand.Id = id;
            await _commandDispatcher.SendAsync(removeCommentCommand).ConfigureAwait(false);
            return StatusCode(StatusCodes.Status200OK, new BaseResponse()
            {
                Message = $"{removeCommentCommand.CommentId} Comment  has been deleted"
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
            const string SAFE_ERROR_MESSAGE = "Error while processing request to remove comment";
            _logger.Log(LogLevel.Error, exception, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
            {
                Message = SAFE_ERROR_MESSAGE,
            });
        }
    }
}