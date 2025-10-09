using Application.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AiConversationsController  : ControllerBase
    {
        private readonly ILogger<AiConversationsController> _logger;
        private readonly IAiConversationSessionService _aiConversationSessionService;
        private readonly IAiChatMessageService _chatService;
        public AiConversationsController(ILogger<AiConversationsController> logger, IAiConversationSessionService aiConversationSessionService, IAiChatMessageService chatService)
        {
            _logger = logger;
            _aiConversationSessionService = aiConversationSessionService;
            _chatService = chatService;
        }
        [HttpPost("create-session")]

    }
}