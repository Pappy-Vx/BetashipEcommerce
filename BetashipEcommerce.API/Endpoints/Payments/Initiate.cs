using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Payments.InitiatePayment;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Payments;

public sealed class Initiate : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Payments.Initiate, HandleAsync)
           .WithName("InitiatePayment")
           .WithTags("Payments")
           .WithSummary("Initiate a payment for an order")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces<Guid>(StatusCodes.Status201Created)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        InitiatePaymentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new InitiatePaymentCommand(
            request.OrderId,
            request.CustomerId,
            request.Amount,
            request.Currency,
            request.PaymentMethod);

        var result = await sender.Send(command, cancellationToken);

        return result.ToCreatedResult(
            "GetOrderById",
            new { orderId = request.OrderId });
    }
}

public sealed record InitiatePaymentRequest(
    Guid OrderId,
    Guid CustomerId,
    decimal Amount,
    string Currency,
    int PaymentMethod);
