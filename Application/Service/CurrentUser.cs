using System.Net;
using System.Security.Claims;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Http;

namespace Application.Service
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _http;
        public CurrentUser(IHttpContextAccessor http)
        {
            _http = http;
        }

        public Guid? GetCurrentUserId()
        {
            var id = _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var parsed) || parsed == Guid.Empty)
            {
                return null;
            }
            return parsed;
        }
    }
}