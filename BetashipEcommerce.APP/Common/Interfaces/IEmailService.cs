namespace BetashipEcommerce.APP.Common.Interfaces;

/// <summary>
/// Email notification service abstraction.
/// Implemented in the infrastructure/API layer (e.g., SendGrid, SMTP).
/// </summary>
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);

    Task SendTemplatedAsync(
        string to,
        string templateId,
        object templateData,
        CancellationToken cancellationToken = default);
}
