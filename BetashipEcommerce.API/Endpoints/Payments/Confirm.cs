using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Payments.ProcessPayment;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Payments;

/// <summary>
/// Webhook endpoint for payment gateway callbacks confirming payment success.
/// Internally delegates to ProcessPaymentCommand to complete the payment lifecycle.
/// </summary>
public sealed class Confirm : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Payments.Confirm, HandleAsync)
           .WithName("ConfirmPayment")
           .WithTags("Payments")
           .WithSummary("Confirm a payment via gateway webhook callback")
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid paymentId,
        ConfirmPaymentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ProcessPaymentCommand(
            paymentId,
            request.TransactionId,
            request.GatewayResponse);

        var result = await sender.Send(command, cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record ConfirmPaymentRequest(string TransactionId, string? GatewayResponse);
