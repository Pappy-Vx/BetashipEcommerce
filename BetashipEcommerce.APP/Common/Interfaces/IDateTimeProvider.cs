namespace BetashipEcommerce.APP.Common.Interfaces;

/// <summary>
/// DateTime abstraction for testability. Never call DateTime.UtcNow directly in handlers.
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

/// <summary>
/// Default implementation using system clock.
/// </summary>
public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
