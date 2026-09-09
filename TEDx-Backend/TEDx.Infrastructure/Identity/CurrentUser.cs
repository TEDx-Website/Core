using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Identity.Enums;
namespace TEDx.Infrastructure.Identity
{
    public sealed class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId =>
            Guid.TryParse(_httpContextAccessor.HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : null;

        public string? Email =>
            _httpContextAccessor.HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.Email);

        public GlobalRole? Role =>
            _httpContextAccessor.HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.Role) is string roleString && Enum.TryParse<GlobalRole>(roleString, out var role) ? role : null;

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?
                .User?
                .Identity?
                .IsAuthenticated ?? false;
        
        public bool IsAdmin =>
            Role == GlobalRole.Admin;
    }
}
