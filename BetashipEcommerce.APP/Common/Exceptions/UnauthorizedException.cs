namespace BetashipEcommerce.APP.Common.Exceptions;

public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException()
        : base("You are not authenticated.") { }

    public UnauthorizedException(string message)
        : base(message) { }
}
