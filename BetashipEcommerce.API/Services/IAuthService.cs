namespace BetashipEcommerce.API.Services;

public interface IAuthService
{
    Task<AuthTokenResponse?> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthTokenResponse?> RegisterAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthTokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task LogoutAsync(string accessToken, CancellationToken cancellationToken = default);
    Task VerifyEmailAsync(string token, CancellationToken cancellationToken = default);
}

public sealed record AuthTokenResponse(
    string AccessToken,
    string RefreshToken,
    string TokenType,
    int ExpiresIn);
