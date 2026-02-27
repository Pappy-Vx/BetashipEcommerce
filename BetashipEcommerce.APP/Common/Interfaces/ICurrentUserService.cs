namespace BetashipEcommerce.APP.Common.Interfaces;

/// <summary>
/// Provides access to current user information from the request context.
/// Implemented in the API layer via HttpContext.
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
}
