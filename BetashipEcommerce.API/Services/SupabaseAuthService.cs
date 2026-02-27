namespace BetashipEcommerce.API.Services;

/// <summary>
/// Supabase Auth adapter.
/// TODO: Inject Supabase client and implement real auth calls against the Supabase Auth API.
/// </summary>
public sealed class SupabaseAuthService(ILogger<SupabaseAuthService> logger) : IAuthService
{
    public Task<AuthTokenResponse?> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Auth: login attempt for {Email}", email);
        // TODO: call Supabase Auth REST API POST /auth/v1/token?grant_type=password
        throw new NotImplementedException("Supabase auth login not yet implemented.");
    }

    public Task<AuthTokenResponse?> RegisterAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Auth: register attempt for {Email}", email);
        // TODO: call Supabase Auth REST API POST /auth/v1/signup
        throw new NotImplementedException("Supabase auth register not yet implemented.");
    }

    public Task<AuthTokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Auth: token refresh");
        // TODO: call Supabase Auth REST API POST /auth/v1/token?grant_type=refresh_token
        throw new NotImplementedException("Supabase auth token refresh not yet implemented.");
    }

    public Task LogoutAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Auth: logout");
        // TODO: call Supabase Auth REST API POST /auth/v1/logout
        return Task.CompletedTask;
    }

    public Task VerifyEmailAsync(string token, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Auth: email verification");
        // TODO: call Supabase Auth REST API GET /auth/v1/verify?token={token}&type=email
        return Task.CompletedTask;
    }
}
