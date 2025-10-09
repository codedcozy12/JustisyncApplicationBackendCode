using Application.Dto;

namespace Application.Interfaces.Service
{
    public interface IChatService
    {
        Task<ResponseDto<ChatCreateRequestDto>> CreateChatAsync(ChatCreateRequestDto request);

        Task<ResponseDto<ChatDto>> GetChatByIdAsync(Guid chatId);

        Task<ResponseDto<IEnumerable<ChatDto>>> GetChatsByUserIdAsync(Guid userId);

        Task<ResponseDto<string>> DeleteChatAsync(Guid chatId);
    }
}