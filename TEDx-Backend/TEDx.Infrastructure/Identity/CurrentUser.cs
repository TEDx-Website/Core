using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TEDx.Application.Common.Interfaces;

namespace TEDx.Infrastructure.Identity
{
    internal sealed class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId =>
            _httpContextAccessor.HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
