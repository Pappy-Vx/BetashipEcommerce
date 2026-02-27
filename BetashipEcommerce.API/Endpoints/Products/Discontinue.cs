using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Products.DiscontinueProduct;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Products;

public sealed class Discontinue : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Products.Discontinue, HandleAsync)
           .WithName("DiscontinueProduct")
           .WithTags("Products")
           .WithSummary("Discontinue a product")
           .RequireAuthorization(PolicyNames.AdminOnly)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        Guid productId,
        DiscontinueRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DiscontinueProductCommand(productId, request.Reason);
        var result  = await sender.Send(command, cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record DiscontinueRequest(string Reason);
