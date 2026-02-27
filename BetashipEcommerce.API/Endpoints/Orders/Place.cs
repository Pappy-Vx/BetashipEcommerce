using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Orders.PlaceOrder;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Orders;

public sealed class Place : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Orders.Place, HandleAsync)
           .WithName("PlaceOrder")
           .WithTags("Orders")
           .WithSummary("Place a new order")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces<Guid>(StatusCodes.Status201Created)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        PlaceOrderRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var items = request.Items
            .Select(i => new PlaceOrderItemDto(i.ProductId, i.Quantity))
            .ToList()
            .AsReadOnly();

        var command = new PlaceOrderCommand(
            request.CustomerId,
            items,
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

public sealed record PlaceOrderRequest(
    Guid CustomerId,
    IReadOnlyList<OrderItemRequest> Items,
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

public sealed record OrderItemRequest(Guid ProductId, int Quantity);
