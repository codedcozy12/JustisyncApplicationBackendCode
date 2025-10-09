namespace Application.Interfaces.ExternalService
{
    public interface IAIChatProvider
    {
        Task<string> GetChatResponseAsync(string message);
    }
}
