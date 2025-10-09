using Application.Interfaces.ExternalService;
using Infrastructures.Settings;
using Microsoft.Extensions.Options;
using OpenAI.Responses;

namespace Infrastructures.Service
{
    public class AiChatProvider : IAIChatProvider
    {
        private readonly OpenAiSettings _options;

        public AiChatProvider(IOptions<OpenAiSettings> options)
        {
            _options = options.Value;
        }

        public async Task<string> GetChatResponseAsync(string message)
        {
            // Use the API key from settings

            var client = new OpenAIResponseClient(model: "gpt-5", apiKey: _options.ApiKey);

            // Call AI asynchronously
            OpenAIResponse response = await client.CreateResponseAsync(message);

            // Return only the text
            return response.GetOutputText();
        }
    }
}
