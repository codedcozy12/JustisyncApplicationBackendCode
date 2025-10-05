using Application.Dto;
using Application.Interfaces.ExternalService;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class AiChatMessageService : IAiChatMessageService
    {
        private readonly ILogger<AiChatMessageService> _logger;
        private readonly IAiChatMessageRepository _aiChatMessageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAIChatProvider _aIChatProvider;
        public AiChatMessageService(ILogger<AiChatMessageService> logger, IAIChatProvider aIChatProvider, IAiChatMessageRepository aiChatMessageRepository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _aIChatProvider = aIChatProvider;
            _aiChatMessageRepository = aiChatMessageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto<AichatMessageCreateRequestDto>> AddMessageAsync(AichatMessageCreateRequestDto request)
        {
            if (request == null)
            {
                _logger.LogError("AI Chat Message creation request is null or invalid");
                return new ResponseDto<AichatMessageCreateRequestDto>
                {
                    IsSuccess = false,
                    Message = "Invalid AI Chat Message creation request",
                    Data = null
                };
            }
            _logger.LogInformation("Creating new AI Chat Message for SessionId: {SessionId} and UserId: {UserId}");
            var message = new AiChatMessage
            {
                Id = Guid.NewGuid(),
                Message = request.Message
            };
            await _aiChatMessageRepository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("AI Chat Message created successfully with Id: {MessageId}", message.Id);
            return new ResponseDto<AichatMessageCreateRequestDto>
            {
                IsSuccess = true,
                Message = "AI Chat Message created successfully",
                Data = request
            };
            string Prompt(string message, string role)
            {
                return $@"
                You are an AI assistant for JustiSync, a digital legal solution platform. 
                Your role is to assist users based on their role and context within the application. 

                User Role: {role}
                User Message: ""{message}""

                Guidelines for your response:
                1. If the user is a Client, provide guidance on case processes, status, and general explanations without giving real legal advice.
                2. If the user is a Lawyer, provide legal insights, precedent comparisons, document summaries, or suggestions relevant to their case.
                3. If the user is a Judge, provide context, summaries, or clarifications related to case proceedings only.
                4. Respond professionally, politely, and clearly.
                5. Use the context of JustiSync entities: cases, lawyers, clients, judges, court sessions, documents, and AI-assisted legal analysis.
                6. Never provide actual legal advice; always remind clients to consult a licensed lawyer.
                ";
            }
       }
        public async Task<ResponseDto<IEnumerable<AiChatMessageDto>>> GetAllAsync()
        {
            var messages = await _aiChatMessageRepository.GetAllAsync();
            if (messages == null || !messages.Any())
            {
                _logger.LogWarning("No AI Chat Messages found in the repository");
                return new ResponseDto<IEnumerable<AiChatMessageDto>>
                {
                    IsSuccess = false,
                    Message = "No AI Chat Messages found",
                    Data = Enumerable.Empty<AiChatMessageDto>()
                };
            }
            var messageDtos = messages.Select(m => new AiChatMessageDto
            {
                Id = m.Id,
                AIConversationSessionId = m.AIConversationSessionId,
                Sender = m.Sender,
                Message = m.Message,
                SentAt = m.SentAt
            }).ToList();
            _logger.LogInformation("Retrieved {Count} AI Chat Messages from the repository", messageDtos.Count);
            return new ResponseDto<IEnumerable<AiChatMessageDto>>
            {
                IsSuccess = true,
                Message = "AI Chat Messages retrieved successfully",
                Data = messageDtos
            };
        }
        public async Task<ResponseDto<AiChatMessageDto>> GetByIdAsync(Guid id)
        {
            var message = await _aiChatMessageRepository.GetByIdAsync(id);
            if (message == null)
            {
                _logger.LogWarning("AI Chat Message with Id: {MessageId} not found", id);
                return new ResponseDto<AiChatMessageDto>
                {
                    IsSuccess = false,
                    Message = "AI Chat Message not found",
                    Data = null
                };
            }
            var messageDto = new AiChatMessageDto
            {
                Id = message.Id,
                AIConversationSessionId = message.AIConversationSessionId,
                Sender = message.Sender,
                Message = message.Message,
                SentAt = message.SentAt
            };
            _logger.LogInformation("Retrieved AI Chat Message with Id: {MessageId}", id);
            return new ResponseDto<AiChatMessageDto>
            {
                IsSuccess = true,
                Message = "AI Chat Message retrieved successfully",
                Data = messageDto
            };
        }
        public async Task<ResponseDto<AichatMessageUpdateRequestDto>> UpdateMessageAsync(AiChatMessage message)
        {
            if (message == null)
            {
                _logger.LogError("AI Chat Message update request is null or invalid");
                return new ResponseDto<AichatMessageUpdateRequestDto>
                {
                    IsSuccess = false,
                    Message = "Invalid AI Chat Message update request",
                    Data = null
                };
            }
            var existingMessage = await _aiChatMessageRepository.GetByIdAsync(message.Id);
            if (existingMessage == null)
            {
                _logger.LogWarning("AI Chat Message with Id: {MessageId} not found for update", message.Id);
                return new ResponseDto<AichatMessageUpdateRequestDto>
                {
                    IsSuccess = false,
                    Message = "AI Chat Message not found",
                    Data = null
                };
            }
            existingMessage.Message = message.Message;
            existingMessage.Sender = message.Sender;
            await _aiChatMessageRepository.UpdateAsync(existingMessage);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("AI Chat Message with Id: {MessageId} updated successfully", message.Id);
            var updateRequestDto = new AichatMessageUpdateRequestDto
            {
                Id = existingMessage.Id,
                Message = existingMessage.Message,
            };
            return new ResponseDto<AichatMessageUpdateRequestDto>
            {
                IsSuccess = true,
                Message = "AI Chat Message updated successfully",
                Data = updateRequestDto
            };
        }
        public async Task<ResponseDto<AiChatMessageDto>> GetLastMessageInSessionAsync(Guid sessionId)
        {
            var message = await _aiChatMessageRepository.GetLastMessageInSessionAsync(sessionId);
            if (message == null)
            {
                _logger.LogWarning("No AI Chat Messages found for SessionId: {SessionId}", sessionId);
                return new ResponseDto<AiChatMessageDto>
                {
                    IsSuccess = false,
                    Message = "No AI Chat Messages found for the session",
                    Data = null
                };
            }
            var messageDto = new AiChatMessageDto
            {
                Id = message.Id,
                AIConversationSessionId = message.AIConversationSessionId,
                Sender = message.Sender,
                Message = message.Message,
                SentAt = message.SentAt
            };
            _logger.LogInformation("Retrieved last AI Chat Message for SessionId: {SessionId}", sessionId);
            return new ResponseDto<AiChatMessageDto>
            {
                IsSuccess = true,
                Message = "Last AI Chat Message retrieved successfully",
                Data = messageDto
            };
        }
        public async Task<ResponseDto<IEnumerable<AiChatMessageDto>>> GetMessagesBySessionAsync(Guid sessionId)
        {
            var messages = await _aiChatMessageRepository.GetMessagesBySessionAsync(sessionId);
            if (messages == null || !messages.Any())
            {
                _logger.LogWarning("No AI Chat Messages found for SessionId: {SessionId}", sessionId);
                return new ResponseDto<IEnumerable<AiChatMessageDto>>
                {
                    IsSuccess = false,
                    Message = "No AI Chat Messages found for the session",
                    Data = Enumerable.Empty<AiChatMessageDto>()
                };
            }
            var messageDtos = messages.Select(m => new AiChatMessageDto
            {
                Id = m.Id,
                AIConversationSessionId = m.AIConversationSessionId,
                Sender = m.Sender,
                Message = m.Message,
                SentAt = m.SentAt
            }).ToList();
            _logger.LogInformation("Retrieved {Count} AI Chat Messages for SessionId: {SessionId}", messageDtos.Count, sessionId);
            return new ResponseDto<IEnumerable<AiChatMessageDto>>
            {
                IsSuccess = true,
                Message = "AI Chat Messages retrieved successfully for the session",
                Data = messageDtos
            };
        }
        public async Task<ResponseDto<IEnumerable<AiChatMessageDto>>> GetMessagesByUserAsync(Guid userId)
        {
            var messages = await _aiChatMessageRepository.GetMessagesByUserAsync(userId);
            if (messages == null || !messages.Any())
            {
                _logger.LogError("No Ai chat message found for UserId: {UserId}", userId);
                return new ResponseDto<IEnumerable<AiChatMessageDto>>
                {
                    IsSuccess = false,
                    Message = "No AI Chat Messages found for this user",
                    Data = Enumerable.Empty<AiChatMessageDto>()
                };
            }
            var messagesDtos = messages.Select(m => new AiChatMessageDto
            {
                Id = m.Id,
                Message = m.Message,
                SentAt = m.SentAt
            })
            .ToList();

            return new ResponseDto<IEnumerable<AiChatMessageDto>>
            {
                IsSuccess = true,
                Message = "AI Chat Messages retrieved successfully for the user",
                Data = messagesDtos
            };
        }
        public async Task<ResponseDto<IEnumerable<AiChatMessageDto>>> GetMessagesInTimeRangeAsync(Guid sessionId, DateTime start, DateTime end)
        {
        }
    }
}