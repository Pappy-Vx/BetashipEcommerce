using BetashipEcommerce.API.Configuration;
using BetashipEcommerce.API.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BetashipEcommerce.API.Endpoints.Dev;

/// <summary>
/// DEV ONLY — generates a signed JWT for Swagger testing.
/// This endpoint is not registered in Production.
/// </summary>
public sealed class GenerateToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        if (app is WebApplication webApp && !webApp.Environment.IsDevelopment())
            return;

        app.MapPost("api/dev/token", HandleAsync)
           .WithName("GenerateDevToken")
           .WithTags("Dev (Not in Production)")
           .WithSummary("[DEV ONLY] Generate a signed JWT for Swagger testing")
           .AllowAnonymous()
           .Produces<DevTokenResponse>(StatusCodes.Status200OK);
    }

    private static IResult HandleAsync(
        DevTokenRequest request,
        IOptions<JwtSettings> jwtOptions)
    {
        var settings = jwtOptions.Value;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   request.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, request.Email),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new(ClaimTypes.Name,               request.Email),
            new(ClaimTypes.NameIdentifier,     request.UserId.ToString()),
            new(ClaimTypes.Role,               request.Role)
        };

        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry      = DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes > 0 ? settings.ExpiryMinutes : 60);

        var token = new JwtSecurityToken(
            issuer:             settings.Issuer,
            audience:           settings.Audience,
            claims:             claims,
            notBefore:          DateTime.UtcNow,
            expires:            expiry,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Results.Ok(new DevTokenResponse(
            AccessToken: tokenString,
            TokenType:   "Bearer",
            ExpiresIn:   (int)(expiry - DateTime.UtcNow).TotalSeconds,
            Note:        "DEV ONLY — paste the AccessToken value into Swagger Authorize as: Bearer <token>"));
    }
}

public sealed record DevTokenRequest(
    Guid   UserId = default,
    string Email  = "dev@betaship.com",
    string Role   = "Admin");

public sealed record DevTokenResponse(
    string AccessToken,
    string TokenType,
    int    ExpiresIn,
    string Note);
