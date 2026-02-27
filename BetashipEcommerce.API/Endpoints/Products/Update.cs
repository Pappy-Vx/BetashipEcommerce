using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Products.UpdateProduct;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Products;

public sealed class Update : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Products.Update, HandleAsync)
           .WithName("UpdateProduct")
           .WithTags("Products")
           .WithSummary("Update product details (name, description, SKU)")
           .RequireAuthorization(PolicyNames.AdminOnly)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        Guid productId,
        UpdateProductRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            productId,
            request.Name,
            request.Description,
            request.Sku);

        var result = await sender.Send(command, cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record UpdateProductRequest(string Name, string Description, string Sku);
