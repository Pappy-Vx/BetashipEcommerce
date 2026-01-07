using BetashipEcommerce.CORE.Identity.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BetashipEcommerce.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserId? UserId
        {
            get
            {
                var id = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(id)) return null;
                return Guid.TryParse(id, out var g) ? new UserId(g) : null;
            }
        }

        public string? Username => _httpContextAccessor?.HttpContext?.User?.Identity?.Name;

        public bool IsAuthenticated => _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
