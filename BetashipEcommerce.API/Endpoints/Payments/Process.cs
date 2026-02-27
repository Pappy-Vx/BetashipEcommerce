using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Payments.ProcessPayment;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Payments;

public sealed class Process : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Payments.Process, HandleAsync)
           .WithName("ProcessPayment")
           .WithTags("Payments")
           .WithSummary("Mark a payment as processed with the gateway transaction ID")
           .RequireAuthorization(PolicyNames.AdminOnly)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        Guid paymentId,
        ProcessPaymentRequest request,
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

public sealed record ProcessPaymentRequest(string TransactionId, string? GatewayResponse);
