using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Products.CreateProduct;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Products;

public sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Products.Create, HandleAsync)
           .WithName("CreateProduct")
           .WithTags("Products")
           .WithSummary("Create a new product")
           .RequireAuthorization(PolicyNames.AdminOnly)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces<Guid>(StatusCodes.Status201Created)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> HandleAsync(
        CreateProductRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.Description,
            request.Sku,
            request.Price,
            request.Currency,
            request.Category);

        var result = await sender.Send(command, cancellationToken);

        return result.ToCreatedResult(
            "GetProductById",
            new { productId = result.IsSuccess ? result.Value : Guid.Empty });
    }
}

public sealed record CreateProductRequest(
    string Name,
    string Description,
    string Sku,
    decimal Price,
    string Currency,
    int Category);
