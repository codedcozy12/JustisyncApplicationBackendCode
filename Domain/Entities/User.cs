using System.Security;
using Domain.Enums;

namespace Domain.Entities
{
    public class User : BaseEntities
    {
            public required string Username { get; set; }
            public required string Email { get; set; }
            public required string PasswordHash { get; set; }
            public required string FirstName { get; set; }
            public required string LastName { get; set; }
            public required string MiddleName { get; set; }
            public required string PhoneNumber { get; set; }
            public Role Role { get; set; }
            public string? ProfilePictureUrl { get; set; }
            public bool IsEmailConfirmed { get; set; } = false;
            public ICollection<Notification> Notifications { get; set; } = [];
            public ICollection<Chat> UserChats { get; set; } = [];
            public ICollection<ChatMessage> SentMessages { get; set; } = [];
            public ICollection<ChatMessage> ReceivedMessages { get; set; } = [];
            public Accountstatus Accountstatus { get; set; }
            public ICollection<AiConversationSession> AIConversationSessions { get; set; } = [];
        }
    }