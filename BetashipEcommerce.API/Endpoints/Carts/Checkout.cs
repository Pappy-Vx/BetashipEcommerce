using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Carts.CheckoutCart;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Carts;

public sealed class Checkout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Carts.Checkout, HandleAsync)
           .WithName("CheckoutCart")
           .WithTags("Carts")
           .WithSummary("Convert the cart into an order and move it to the database")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces<Guid>(StatusCodes.Status201Created)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid customerId,
        CheckoutCartRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CheckoutCartCommand(
            customerId,
            request.ShippingStreet,
            request.ShippingCity,
            request.ShippingState,
            request.ShippingCountry,
            request.ShippingPostalCode,
            request.BillingStreet,
            request.BillingCity,
            request.BillingState,
            request.BillingCountry,
            request.BillingPostalCode);

        var result = await sender.Send(command, cancellationToken);

        return result.ToCreatedResult(
            "GetOrderById",
            new { orderId = result.IsSuccess ? result.Value : Guid.Empty });
    }
}

public sealed record CheckoutCartRequest(
    string ShippingStreet,
    string ShippingCity,
    string ShippingState,
    string ShippingCountry,
    string ShippingPostalCode,
    string BillingStreet,
    string BillingCity,
    string BillingState,
    string BillingCountry,
    string BillingPostalCode);
