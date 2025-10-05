using Application.Dto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class AiConversationSessionService : IAiConversationSessionService
    {
        private readonly ILogger<AiConversationSessionService> _logger;
        private readonly IAiConversationSessionRepository _aiConversationSessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AiConversationSessionService(ILogger<AiConversationSessionService> logger, IAiConversationSessionRepository aiConversationSessionRepository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _aiConversationSessionRepository = aiConversationSessionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto<AiConversationSessionCreateRequestDto>> CreateSessionAsync(AiConversationSessionCreateRequestDto request)
        {
            if (request == null || request.UserId == Guid.Empty)
            {
                _logger.LogError("AI Conversation Session creation request is null or invalid");
                return new ResponseDto<AiConversationSessionCreateRequestDto>
                {
                    IsSuccess = false,
                    Message = "Invalid AI Conversation Session creation request",
                    Data = null
                };
            }
            _logger.LogInformation("Creating new AI Conversation Session for UserId: {UserId}", request.UserId);
            var session = new AiConversationSession
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId
            };
            await _aiConversationSessionRepository.AddAsync(session);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("AI Conversation Session created successfully with Id: {SessionId}", session.Id);
            return new ResponseDto<AiConversationSessionCreateRequestDto>
            {
                IsSuccess = true,
                Message = "AI Conversation Session created successfully",
                Data = request
            };
        }
        public async Task DeleteSessionAsync(Guid sessionId)
        {
            var session = await _aiConversationSessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                _logger.LogWarning("AI Conversation Session deletion failed: Session with Id {SessionId} not found", sessionId);
                return;
            }
            await _aiConversationSessionRepository.UpdateAsync(session);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("AI Conversation Session with Id {SessionId} deleted successfully", sessionId);
        }

        public async Task<ResponseDto<AiConversationSessionDto>> GetSessionByIdAsync(Guid sessionId)
        {
            var session = await _aiConversationSessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                _logger.LogWarning("AI Conversation Session with Id: {SessionId} not found", sessionId);
                return new ResponseDto<AiConversationSessionDto>
                {
                    IsSuccess = false,
                    Message = "AI Conversation Session not found",
                    Data = null
                };
            }
            var sessionDto = new AiConversationSessionDto
            {
                Id = session.Id,
                UserId = session.UserId
            };
            return new ResponseDto<AiConversationSessionDto>
            {
                IsSuccess = true,
                Message = "AI Conversation Session retrieved successfully",
                Data = sessionDto
            };
        }
        public async Task<ResponseDto<IEnumerable<AiConversationSessionDto>>> GetSessionsByUserIdAsync(Guid userId)
        {
            var sessions = await _aiConversationSessionRepository.GetAllAsync();
            var filteredSessions = sessions.Where(s => s.UserId == userId).ToList();

            if (!filteredSessions.Any())
            {
                _logger.LogInformation("No AI Conversation Sessions found for UserId: {UserId}", userId);
                return new ResponseDto<IEnumerable<AiConversationSessionDto>>
                {
                    IsSuccess = false,
                    Message = "No AI Conversation Sessions found for the user",
                    Data = Enumerable.Empty<AiConversationSessionDto>()
                };
            }
            var sessionDtos = filteredSessions.Select(s => new AiConversationSessionDto
            {
                Id = s.Id,
                UserId = s.UserId
            });
            return new ResponseDto<IEnumerable<AiConversationSessionDto>>
            {
                IsSuccess = true,
                Message = "AI Conversation Sessions retrieved successfully",
                Data = sessionDtos
            };
        }
    }
}
