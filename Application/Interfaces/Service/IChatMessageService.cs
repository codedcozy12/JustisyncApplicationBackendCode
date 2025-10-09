using Application.Dto;

namespace Application.Interfaces.Service
{
    public interface IChatMessageService
    {
        Task<ResponseDto<ChatMessageCreateRequestDto>> SendMessageAsync(ChatMessageCreateRequestDto request);

        Task<ResponseDto<IEnumerable<ChatMessageDto>>> GetAllChatMessagesAsync(Guid chatId);

        Task<ResponseDto<ChatMessageDto>> GetMessageByIdAsync(Guid messageId);
        Task<ResponseDto<string>> MarkAllChatAsReadAsync(Guid chatId, Guid userId);
    }
}