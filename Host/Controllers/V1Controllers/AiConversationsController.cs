using Application.Dto;
using Application.Interfaces.Service;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AiConversationsController : ControllerBase
    {
        private readonly ILogger<AiConversationsController> _logger;
        private readonly IAiConversationSessionService _aiConversationSessionService;
        private readonly IAiChatMessageService _aiChatMessageService;
        public AiConversationsController(ILogger<AiConversationsController> logger, IAiConversationSessionService aiConversationSessionService, IAiChatMessageService aiChatMessageService)
        {
            _logger = logger;
            _aiConversationSessionService = aiConversationSessionService;
            _aiChatMessageService = aiChatMessageService;
        }
        [HttpGet("messages/{id:guid}")]
        [SwaggerOperation(Summary = "Get a single AI chat message by ID", Description = "Retrieves a single chat message based on its unique identifier.")]
        public async Task<IActionResult> GetMessageById(Guid id)
        {
            var result = await _aiChatMessageService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("messages")]
        [SwaggerOperation(Summary = "Get all AI chat messages", Description = "Retrieves all chat messages from the database.")]
        public async Task<IActionResult> GetAllMessages()
        {
            var result = await _aiChatMessageService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost("messages")]
        [SwaggerOperation(Summary = "Add a new chat message", Description = "Saves a new chat message under an existing AI conversation session.")]
        public async Task<IActionResult> AddMessage([FromBody] AichatMessageCreateRequestDto request)
        {
            var result = await _aiChatMessageService.AddMessageAsync(request);
            return Ok(result);
        }

        [HttpPut("messages")]
        [SwaggerOperation(Summary = "Update a chat message", Description = "Modifies the text content of an existing chat message.")]
        public async Task<IActionResult> UpdateMessage([FromBody] AichatMessageUpdateRequestDto request)
        {
            var messageEntity = new AiChatMessage
            {
                Id = request.Id,
                Message = request.Message
            };

            var result = await _aiChatMessageService.UpdateMessageAsync(messageEntity);
            return Ok(result);
        }

        [HttpGet("messages/session/{sessionId:guid}")]
        [SwaggerOperation(Summary = "Get all messages in a session", Description = "Retrieves all chat messages associated with a specific AI conversation session.")]
        public async Task<IActionResult> GetMessagesBySession(Guid sessionId)
        {
            var result = await _aiChatMessageService.GetMessagesBySessionAsync(sessionId);
            return Ok(result);
        }

        [HttpGet("messages/user/{userId:guid}")]
        [SwaggerOperation(Summary = "Get messages by user", Description = "Retrieves all chat messages sent or received by a specific user.")]
        public async Task<IActionResult> GetMessagesByUser(Guid userId)
        {
            var result = await _aiChatMessageService.GetMessagesByUserAsync(userId);
            return Ok(result);
        }

        [HttpGet("messages/session/{sessionId:guid}/range")]
        [SwaggerOperation(Summary = "Get messages within time range", Description = "Retrieves all chat messages within a specified date range for a session.")]
        public async Task<IActionResult> GetMessagesInTimeRange(Guid sessionId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var result = await _aiChatMessageService.GetMessagesInTimeRangeAsync(sessionId, start, end);
            return Ok(result);
        }

        [HttpGet("messages/session/{sessionId:guid}/last")]
        [SwaggerOperation(Summary = "Get last message in session", Description = "Retrieves the most recent chat message in a specific conversation session.")]
        public async Task<IActionResult> GetLastMessageInSession(Guid sessionId)
        {
            var result = await _aiChatMessageService.GetLastMessageInSessionAsync(sessionId);
            return Ok(result);
        }

        [HttpPost("sessions")]
        [SwaggerOperation(Summary = "Create a new conversation session", Description = "Starts a new AI conversation session for a specific user.")]
        public async Task<IActionResult> CreateSession([FromBody] AiConversationSessionCreateRequestDto request)
        {
            var result = await _aiConversationSessionService.CreateSessionAsync(request);
            return Ok(result);
        }

        [HttpGet("sessions/{sessionId:guid}")]
        [SwaggerOperation(Summary = "Get session by ID", Description = "Retrieves details of a specific AI conversation session by ID.")]
        public async Task<IActionResult> GetSessionById(Guid sessionId)
        {
            var result = await _aiConversationSessionService.GetSessionByIdAsync(sessionId);
            return Ok(result);
        }
        [HttpGet("sessions/user/{userId:guid}")]
        [SwaggerOperation(Summary = "Get sessions by user", Description = "Retrieves all AI conversation sessions belonging to a specific user.")]
        public async Task<IActionResult> GetSessionsByUser(Guid userId)
        {
            var result = await _aiConversationSessionService.GetSessionsByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpDelete("sessions/{sessionId:guid}")]
        [SwaggerOperation(Summary = "Delete conversation session", Description = "Deletes a specific AI conversation session and all related messages.")]
        public async Task<IActionResult> DeleteSession(Guid sessionId)
        {
            await _aiConversationSessionService.DeleteSessionAsync(sessionId);
            _logger.LogInformation("AI conversation session {SessionId} deleted successfully.", sessionId);
            return NoContent();
        }
    }
}