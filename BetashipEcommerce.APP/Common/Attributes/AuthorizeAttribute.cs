namespace BetashipEcommerce.APP.Common.Attributes;

/// <summary>
/// Marks a MediatR request as requiring authentication.
/// Optionally specifies required roles (comma-separated).
/// Used by AuthorizationBehaviour to enforce access control.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class AuthorizeAttribute : Attribute
{
    /// <summary>
    /// Comma-separated list of required roles. Leave empty to require authentication only.
    /// Example: "Admin,Manager"
    /// </summary>
    public string? Roles { get; set; }
}
