using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Products.UpdateProductPrice;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Products;

public sealed class UpdatePrice : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Products.UpdatePrice, HandleAsync)
           .WithName("UpdateProductPrice")
           .WithTags("Products")
           .WithSummary("Update the price of a product")
           .RequireAuthorization(PolicyNames.AdminOnly)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        Guid productId,
        UpdatePriceRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductPriceCommand(productId, request.NewPrice, request.Currency);
        var result  = await sender.Send(command, cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record UpdatePriceRequest(decimal NewPrice, string Currency);
