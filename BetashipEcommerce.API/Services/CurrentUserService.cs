using BetashipEcommerce.CORE.Identity.ValueObjects;
using System.Security.Claims;

using AppInterface  = BetashipEcommerce.APP.Common.Interfaces.ICurrentUserService;
using CoreInterface = BetashipEcommerce.CORE.Repositories.ICurrentUserService;

namespace BetashipEcommerce.API.Services;

/// <summary>
/// Single implementation satisfying both ICurrentUserService contracts:
///   - CORE.Repositories.ICurrentUserService  (used by DAL interceptors / audit columns)
///   - APP.Common.Interfaces.ICurrentUserService (used by MediatR pipeline behaviours)
///
/// Both interfaces declare UserId with different types (value-object vs Guid?), so
/// the CORE member is implemented explicitly to avoid the ambiguous property error.
/// </summary>
public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)
    : CoreInterface, AppInterface
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    private Guid? RawUserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var g) ? g : null;
        }
    }

    // ── CORE.Repositories.ICurrentUserService ────────────────────────────────

    UserId? CoreInterface.UserId =>
        RawUserId is { } g ? new UserId(g) : null;

    string? CoreInterface.Username =>
        User?.Identity?.Name;

    // ── APP.Common.Interfaces.ICurrentUserService ─────────────────────────────

    Guid? AppInterface.UserId => RawUserId;

    string? AppInterface.Email =>
        User?.FindFirstValue(ClaimTypes.Email)
        ?? User?.FindFirstValue("email");

    IEnumerable<string> AppInterface.Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value)
        ?? Enumerable.Empty<string>();

    // ── Shared (identical on both interfaces) ────────────────────────────────

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;
}
