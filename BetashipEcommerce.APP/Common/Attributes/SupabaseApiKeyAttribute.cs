namespace BetashipEcommerce.APP.Common.Attributes;

/// <summary>
/// Marks a MediatR request as requiring a valid Supabase service API key.
/// Used for internal/admin operations that bypass user-level JWT authentication.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class SupabaseApiKeyAttribute : Attribute
{
    /// <summary>
    /// Optional description of why the service key is required.
    /// </summary>
    public string? Reason { get; set; }
}
