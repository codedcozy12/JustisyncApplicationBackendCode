using Application.Dto;
using Application.Interface;
using Application.Interfaces.ExternalService;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class AiChatMessageService : IAiChatMessageService
    {
        private readonly ILogger<AiChatMessageService> _logger;
        private readonly IAiChatMessageRepository _aiChatMessageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IUserRepository _userRepository;
        private readonly IAIChatProvider _aIChatProvider;
        public AiChatMessageService(ILogger<AiChatMessageService> logger,IUserRepository userRepository,ICurrentUser currentUser, IAIChatProvider aIChatProvider, IAiChatMessageRepository aiChatMessageRepository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _currentUser = currentUser;
            _userRepository = userRepository;
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
            _logger.LogInformation("Creating new AI Chat Message for SessionId: {SessionId} and UserId: {UserId}",request.AIConversationSessionId,request.Sender);
            var userId = _currentUser.GetCurrentUserId();
            if (userId == null)
            {
                return new ResponseDto<AichatMessageCreateRequestDto>
                {
                    IsSuccess = false,
                    Message = "User Not Authenticated",
                    Data = null
                };
            }
            var user = await _userRepository.GetByIdAsync(userId.Value);
            if (user == null)
            {
                return new ResponseDto<AichatMessageCreateRequestDto>
                {
                    IsSuccess = false,
                    Message = "User Not Found",
                    Data = null
                };
            }
            var message = new AiChatMessage
            {
                Id = Guid.NewGuid(),
                Message = request.Message,
                Sender = AIMessageSender.User,
                AIConversationSessionId = request.AIConversationSessionId
            };

            string prompt = Prompt(request.Message, user.Role.ToString(), user.FirstName + " " + user.LastName);
            var response = await _aIChatProvider.GetChatResponseAsync(prompt);
            var aiMessage = new AiChatMessage
            {
                Id = Guid.NewGuid(),
                Message = response ?? "Service Not Available",
                Sender = AIMessageSender.AI,
                AIConversationSessionId = request.AIConversationSessionId
            };
          
            await _aiChatMessageRepository.AddAsync(message);
            await _aiChatMessageRepository.AddAsync(aiMessage);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("AI Chat Message created successfully with Id: {MessageId}", message.Id);

            
            return new ResponseDto<AichatMessageCreateRequestDto>
            {
                IsSuccess = true,
                Message = "AI Chat Message created successfully",
                Data = new AichatMessageCreateRequestDto
                {
                    AIConversationSessionId = request.AIConversationSessionId,
                    Message = aiMessage.Message,
                    Sender = AIMessageSender.AI
                }
            };
            
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
            var messages = await _aiChatMessageRepository.GetMessagesInTimeRangeAsync(sessionId, start, end);
            if (!messages.Any())
            {
                _logger.LogWarning("No AI chat messages found for session {SessionId} between {Start} and {End}", sessionId, start, end);
                return ResponseDto<IEnumerable<AiChatMessageDto>>.Fail("No messages found in the specified time range.");
            }
            var messagesDtos = messages.Select(m => new AiChatMessageDto
                {
                    Id = m.Id,
                    AIConversationSessionId = m.AIConversationSessionId,
                    Sender = m.Sender,
                    Message = m.Message,
                    SentAt = m.SentAt
                }).ToList();

                 _logger.LogInformation("Retrieved messages for session {SessionId} between {Start} and {End}, {Count}", messagesDtos.Count, sessionId, start, end);

                return ResponseDto<IEnumerable<AiChatMessageDto>>.Success(messagesDtos, "Messages retrieved successfully.");
        }
        private string Prompt(string message, string role, string name)
        {
            return $@"
        You are JustiAI, an intelligent legal assistant integrated into the JustiSync digital legal solution platform. 
        You are trained exclusively on law, constitution, judicial procedure, and related legal principles. 
        You must ignore or reject any topic that is not directly related to law, legal processes, or constitutional matters.

        User Role: {role}
        User Name: {name}
        User Message: ""{message}""

        === Response Guidelines ===
        1. You only discuss and analyze legal, constitutional, or procedural matters. 
           If a user’s message is unrelated to law, reply:
           > ""I am a legal domain model. I can only discuss matters related to law, constitution, and judicial processes.""
        
        2. Respond according to user roles:
           - **Client:** Explain general legal processes, rights, or case progress, but never give real legal advice.
             Always remind clients to consult a licensed lawyer for official representation.
           - **Lawyer:** Provide legal insights, relevant case precedents, document or evidence summaries, 
             and procedural guidance consistent with the constitution or applicable law.
           - **Judge:** Provide case summaries, constitutional clarifications, or objective context for proceedings 
             without suggesting judgments or personal opinions.
           - **Registrar or Admin (if applicable):** Provide system-related or procedural clarifications regarding court operations.
        
        3. Maintain a professional, neutral, and respectful tone.
        4. Support your reasoning with legal principles, constitutional sections, or recognized doctrines whenever possible.
        5. You are not permitted to discuss entertainment, politics, technology, personal life, or any topic outside the scope of law.
        6. Structure your answer where applicable as:
           - Legal Issue
           - Applicable Law or Section
           - Explanation
           - Conclusion

        Remember: You are not providing legal advice; you are providing legal understanding and context. ";
        }
    }
}