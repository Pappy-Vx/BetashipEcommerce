using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Payments.RefundPayment;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Payments;

public sealed class Refund : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Payments.Refund, HandleAsync)
           .WithName("RefundPayment")
           .WithTags("Payments")
           .WithSummary("Issue a full or partial refund for a completed payment")
           .RequireAuthorization(PolicyNames.AdminOnly)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        Guid paymentId,
        RefundPaymentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RefundPaymentCommand(
            paymentId,
            request.RefundAmount,
            request.Currency,
            request.Reason);

        var result = await sender.Send(command, cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record RefundPaymentRequest(decimal RefundAmount, string Currency, string Reason);
