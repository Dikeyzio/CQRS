using CQRS.Core.Infostructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Command;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class AddCommentController : ControllerBase
{
    private readonly ILogger<AddCommentController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public AddCommentController(ICommandDispatcher commandDispatcher, ILogger<AddCommentController> logger)
    {
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AddComment(Guid id, [FromBody] AddCommentCommand command)
    {
        try
        {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new BaseResponse()
            {
                Message = $"{id} Comment  has been created"
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
            const string SAFE_ERROR_MESSAGE = "Error while processing request to create new comment";
            _logger.Log(LogLevel.Error, exception, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
            {
                Message = SAFE_ERROR_MESSAGE,
            });
        }

        
    }
}