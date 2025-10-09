using Microsoft.AspNetCore.Mvc;
using Application.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.Service;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers

{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IChatMessageService _chatMessageService;
        private readonly ILogger<ChatsController> _logger;

        public ChatsController(IChatService chatService, IChatMessageService chatMessageService, ILogger<ChatsController> logger)
        {
            _chatService = chatService;
            _chatMessageService = chatMessageService;
            _logger = logger;
        }

        [HttpPost("create")]

        [SwaggerOperation(Summary = "Create a new chat", Description = "Creates a new chat between current user and recipient.")]
        public async Task<IActionResult> CreateChat([FromBody] ChatCreateRequestDto request)
        {
            var response = await _chatService.CreateChatAsync(request);
            return Ok(response);
        }

        [HttpGet("{chatId}")]
        [SwaggerOperation(Summary = "Get chat by Id", Description = "Retrieve a chat by its Id.")]
        public async Task<IActionResult> GetChatById(Guid chatId)
        {
            var response = await _chatService.GetChatByIdAsync(chatId);
            return Ok(response);
        }

        [HttpGet("user/{userId}")]
        [SwaggerOperation(Summary = "Get chats by user Id", Description = "Retrieve all chats for a given user.")]
        public async Task<IActionResult> GetChatsByUserId(Guid userId)
        {
            var response = await _chatService.GetChatsByUserIdAsync(userId);
            return Ok(response);
        }

        [HttpDelete("{chatId}")]
        [SwaggerOperation(Summary = "Delete a chat", Description = "Delete a chat by Id.")]
        public async Task<IActionResult> DeleteChat(Guid chatId)
        {
            var response = await _chatService.DeleteChatAsync(chatId);
            return Ok(response);
        }

        [HttpPost("message/send")]
        [SwaggerOperation(Summary = "Send a chat message", Description = "Send a new message in a specific chat.")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageCreateRequestDto request)
        {
            var response = await _chatMessageService.SendMessageAsync(request);
            return Ok(response);
        }

        [HttpGet("messages/{chatId}")]
        [SwaggerOperation(Summary = "Get all chat messages", Description = "Retrieve all messages for a specific chat.")]
        public async Task<IActionResult> GetAllChatMessages(Guid chatId)
        {
            var response = await _chatMessageService.GetAllChatMessagesAsync(chatId);
            return Ok(response);
        }

        [HttpPatch("message/edit")]
        [SwaggerOperation(Summary = "Edit a chat message", Description = "Edit an existing chat message.")]
        public async Task<IActionResult> MarkChatAsRead([FromBody] Guid chatId, Guid UserId)
        {
            var response = await _chatMessageService.MarkAllChatAsReadAsync(chatId, UserId);
            return Ok(response);
        }

        [HttpGet("message/{messageId}")]
        [SwaggerOperation(Summary = "Get a chat message by Id", Description = "Retrieve a single chat message by Id.")]
        public async Task<IActionResult> GetMessageById(Guid messageId)
        {
            var response = await _chatMessageService.GetMessageByIdAsync(messageId);
            return Ok(response);
        }
    }
}