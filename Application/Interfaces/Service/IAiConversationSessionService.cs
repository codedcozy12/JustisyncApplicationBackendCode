using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces.Service
{
    public interface IAiConversationSessionService
    {
        Task<ResponseDto<AiConversationSessionCreateRequestDto>> CreateSessionAsync(AiConversationSessionCreateRequestDto request);
        Task<ResponseDto<AiConversationSessionDto>> GetSessionByIdAsync(Guid sessionId);
        Task<ResponseDto<IEnumerable<AiConversationSessionDto>>> GetSessionsByUserIdAsync(Guid userId);
        Task DeleteSessionAsync(Guid sessionId);
    }
}