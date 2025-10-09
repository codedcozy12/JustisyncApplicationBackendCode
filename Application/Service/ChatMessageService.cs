using Application.Dto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly ILogger<ChatMessageService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public ChatMessageService(IChatMessageRepository chatMessageRepository, ILogger<ChatMessageService> logger, IUnitOfWork unitOfWork)
        {
            _chatMessageRepository = chatMessageRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseDto<IEnumerable<ChatMessageDto>>> GetAllChatMessagesAsync(Guid chatId)
        {
            var messages = await _chatMessageRepository.GetAllChatMessages(chatId);

            var dtos = messages.Select(m => new ChatMessageDto
            {
                Id = m.Id,
                ChatId = m.ChatId,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Message = m.Message,
                MessageType = m.MessageType,
                SentAt = m.SentAt,
                IsRead = m.IsRead
            });

            return new ResponseDto<IEnumerable<ChatMessageDto>>
            {
                IsSuccess = true,
                Message = "Messages fetched successfully.",
                Data = dtos
            };
        }

        public async Task<ResponseDto<ChatMessageDto>> GetMessageByIdAsync(Guid messageId)
        {
            var message = await _chatMessageRepository.GetMessageByIdAsync(messageId);

            if (message == null)
                return new ResponseDto<ChatMessageDto>
                {
                    IsSuccess = false,
                    Message = "Message not found."
                };

            var dto = new ChatMessageDto
            {
                Id = message.Id,
                ChatId = message.ChatId,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Message = message.Message,
                MessageType = message.MessageType,
                SentAt = message.SentAt,
                IsRead = message.IsRead
            };

            return new ResponseDto<ChatMessageDto>
            {
                IsSuccess = true,
                Message = "Message fetched successfully.",
                Data = dto
            };
        }
        public async Task<ResponseDto<string>> MarkAllChatAsReadAsync(Guid chatId, Guid userId)
        {
            var messages = await _chatMessageRepository.GetMessagesByChatIdAsync(chatId);
            var unread = messages.Where(m => m.ReceiverId == userId && !m.IsRead).ToList();

            foreach (var msg in unread)
            {
                msg.IsRead = true;
                await _chatMessageRepository.AddMessageAsync(msg);
            }

            return new ResponseDto<string>
            {
                IsSuccess = true,
                Message = "All messages marked as read.",
                Data = "Done"
            };
        }
        public async Task<ResponseDto<ChatMessageCreateRequestDto>> SendMessageAsync(ChatMessageCreateRequestDto request)
        {

            var entity = new ChatMessage
            {
                ChatId = request.ChatId,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Message = request.Message,
                MessageType = request.MessageType,
                SentAt = DateTime.UtcNow,
                IsRead = false,
            };

            await _chatMessageRepository.AddMessageAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Message sent: {MessageId} from {SenderId} to {ReceiverId}", entity.Id, entity.SenderId, entity.ReceiverId);

            var dto = new ChatMessageCreateRequestDto
            {
                ChatId = entity.ChatId,
                SenderId = entity.SenderId,
                ReceiverId = entity.ReceiverId,
                Message = entity.Message,
                MessageType = entity.MessageType
            };
            return new ResponseDto<ChatMessageCreateRequestDto>
            {
                IsSuccess = true,
                Message = "Message sent successfully.",
                Data = dto
            };
        }
    }
}