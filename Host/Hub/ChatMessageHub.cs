using Microsoft.AspNetCore.SignalR;
using Domain.Entities;
using Domain.Enums;
using Application.Dto;
using Application.Interfaces.Service;

public class ChatHub : Hub
{
    private readonly IChatMessageService _chatMessageService;

    public ChatHub(IChatMessageService chatMessageService)
    {
        _chatMessageService = chatMessageService;
    }

    public async Task SendMessage(ChatMessageCreateRequestDto request)
    {
        var response = await _chatMessageService.SendMessageAsync(request);
        if (!response.IsSuccess)
        {
            await Clients.Caller.SendAsync("Error", response.Message);
            return;
        }

        await Clients.Group(request.ChatId.ToString()).SendAsync("ReceiveMessage", new
        {
            Id = Guid.NewGuid(),
            ChatId = request.ChatId,
            SenderId = request.SenderId,
            ReceiverId = request.ReceiverId,
            Message = request.Message,
            MessageType = request.MessageType.ToString(),
            SentAt = DateTime.UtcNow,
            IsRead = false
        });
    }
    public async Task MarkChatAsReadAsync(Guid chatId, Guid userId)
    {
        var result = await _chatMessageService.MarkAllChatAsReadAsync(chatId, userId);

        await Clients.Group(chatId.ToString()).SendAsync("MessagesRead", chatId, userId, result.Data);
    }
    public async Task JoinChat(Guid chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());

        var messagesResponse = await _chatMessageService.GetAllChatMessagesAsync(chatId);
        if (messagesResponse.IsSuccess)
        {
            await Clients.Caller.SendAsync("LoadMessages", messagesResponse.Data);
        }
    }
    public async Task LeaveChat(Guid chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
    }
    public async Task MarkMessagesAsRead(Guid chatId, Guid userId)
    {
        await _chatMessageService.MarkAllChatAsReadAsync(chatId, userId);

        await Clients.Group(chatId.ToString()).SendAsync("MessagesRead", userId);
    }
}