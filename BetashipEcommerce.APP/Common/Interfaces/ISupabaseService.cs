namespace BetashipEcommerce.APP.Common.Interfaces;

/// <summary>
/// Supabase service abstraction for interacting with Supabase-specific features
/// such as Auth, Storage, and Realtime.
/// Implemented in the infrastructure/API layer.
/// </summary>
public interface ISupabaseService
{
    Task<string?> GetUserIdFromTokenAsync(string accessToken, CancellationToken cancellationToken = default);

    Task<bool> InvalidateTokenAsync(string accessToken, CancellationToken cancellationToken = default);

    Task<string?> UploadFileAsync(
        string bucket,
        string path,
        byte[] content,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteFileAsync(string bucket, string path, CancellationToken cancellationToken = default);
}
