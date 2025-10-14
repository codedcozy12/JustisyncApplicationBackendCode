using System.Security.Claims;
using Application.Dto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly ILogger<ChatService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public ChatService(IChatRepository chatRepository, ILogger<ChatService> logger, IUnitOfWork unitOfWork)
        {
            _chatRepository = chatRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseDto<ChatCreateRequestDto>> CreateChatAsync(ChatCreateRequestDto request)
        {
             var allChats = await _chatRepository.GetAllChatsAsync();
            var existingChat = allChats.FirstOrDefault(c =>
                    (c.InitiatorId == request.InitiatorId && c.RecipientId == request.RecipientId) ||
                    (c.InitiatorId == request.RecipientId && c.RecipientId == request.InitiatorId));

            if (existingChat is not null)
            {
                _logger.LogWarning("Chat already exists between {InitiatorId} and {RecipientId}",
                    request.InitiatorId, request.RecipientId);

                return new ResponseDto<ChatCreateRequestDto>
                {
                    IsSuccess = false,
                    Message = "Chat already exists between these users."
                };
            }
            var newChat = new Chat
            {
                InitiatorId = request.InitiatorId,
                RecipientId = request.RecipientId,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };
            await _chatRepository.AddChatAsync(newChat);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Chat created successfully with Id: {ChatId}", newChat.Id);

            return new ResponseDto<ChatCreateRequestDto>
            {
                IsSuccess = true,
                Message = "Chat created successfully.",
                Data = new ChatCreateRequestDto
                {
                    RecipientId = request.RecipientId
                }
            };
        }
        public async Task<ResponseDto<string>> DeleteChatAsync(Guid chatId)
        {
            _logger.LogInformation("Attempting to delete chat with Id: {ChatId}", chatId);

            var chat = await _chatRepository.GetChatByIdAsync(chatId);

            if (chat is null)
            {
                _logger.LogWarning("Chat not found for deletion with Id: {ChatId}", chatId);
                return new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Chat not found."
                };
            }

            await _chatRepository.UpdateChatAsync(chat);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Chat deleted successfully with Id: {ChatId}", chatId);

            return new ResponseDto<string>
            {
                IsSuccess = true,
                Message = "Chat deleted successfully.",
                Data = $"Chat {chatId} removed."
            };
        }
        public async Task<ResponseDto<ChatDto>> GetChatByIdAsync(Guid chatId)
        {
            _logger.LogInformation("Retrieving chat with Id: {ChatId}", chatId);

            var chat = await _chatRepository.GetChatByIdAsync(chatId);
            if (chat is null)
            {
                _logger.LogWarning("Chat with Id: {ChatId} is deleted", chatId);
                return new ResponseDto<ChatDto>
                {
                    IsSuccess = false,
                    Message = "Chat not found."
                };
            }

            if (chat is null)
            {
                _logger.LogWarning("Chat not found with Id: {ChatId}", chatId);
                return new ResponseDto<ChatDto>
                {
                    IsSuccess = false,
                    Message = "Chat not found."
                };
            }
            var chatDto = new ChatDto
            {
                Id = chat.Id,
                InitiatorId = chat.InitiatorId,
                RecipientId = chat.RecipientId,
                CreatedAt = chat.CreatedAt,
                LastMessageAt = chat.LastMessageAt,
                Messages = chat.Messages.Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    SenderId = m.SenderId,
                    ReceiverId = m.Receiver.Id,
                    MessageType = m.MessageType,
                    Message = m.Message,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                }).ToList()
            };

            _logger.LogInformation("Chat retrieved successfully with Id: {ChatId}", chatId);

            return new ResponseDto<ChatDto>
            {
                IsSuccess = true,
                Message = "Chat retrieved successfully.",
                Data = chatDto
            };
        }
        public async Task<ResponseDto<IEnumerable<ChatDto>>> GetChatsByUserIdAsync(Guid userId)
        {
            _logger.LogInformation("Fetching all chats for UserId: {UserId}", userId);

            var chats = await _chatRepository.GetChatsByUserIdAsync(userId);

            if (!chats.Any())
            {
                _logger.LogWarning("No chats found for UserId: {UserId}", userId);
                return new ResponseDto<IEnumerable<ChatDto>>
                {
                    IsSuccess = false,
                    Message = "No chats found for this user."
                };
            }

            var chatDtos = chats.Select(chat => new ChatDto
            {
                Id = chat.Id,
                InitiatorId = chat.InitiatorId,
                RecipientId = chat.RecipientId,
                CreatedAt = chat.CreatedAt,
                LastMessageAt = chat.LastMessageAt,
                Messages = chat.Messages.Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    SenderId = m.SenderId,
                    ReceiverId = m.Receiver.Id,
                    MessageType = m.MessageType,
                    Message = m.Message,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                }).ToList()
            }).ToList();

            _logger.LogInformation("{Count} chats found for UserId: {UserId}", chatDtos.Count, userId);

            return new ResponseDto<IEnumerable<ChatDto>>
            {
                IsSuccess = true,
                Message = "Chats retrieved successfully.",
                Data = chatDtos
            };
        }
    }
}