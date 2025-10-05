using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces.Service
{
    public interface IAiChatMessageService
    {
        Task<ResponseDto<AiChatMessageDto>> GetByIdAsync(Guid id);
        Task<ResponseDto<IEnumerable<AiChatMessageDto>>> GetAllAsync();
        Task<ResponseDto<AichatMessageCreateRequestDto>> AddMessageAsync(AichatMessageCreateRequestDto request);
        Task<ResponseDto<AichatMessageUpdateRequestDto>> UpdateMessageAsync(AiChatMessage message);
        Task<ResponseDto<IEnumerable<AiChatMessageDto>>> GetMessagesBySessionAsync(Guid sessionId);
        Task<ResponseDto<IEnumerable<AiChatMessageDto>>> GetMessagesByUserAsync(Guid userId);
        Task<ResponseDto<IEnumerable<AiChatMessageDto>>> GetMessagesInTimeRangeAsync(Guid sessionId, DateTime start, DateTime end);
        Task<ResponseDto<AiChatMessageDto>> GetLastMessageInSessionAsync(Guid sessionId);
    }
}