using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Customers.UpdateCustomerProfile;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Customers;

public sealed class UpdateProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Customers.UpdateProfile, HandleAsync)
           .WithName("UpdateCustomerProfile")
           .WithTags("Customers")
           .WithSummary("Update customer profile details")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid customerId,
        UpdateProfileRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCustomerProfileCommand(
            customerId,
            request.FirstName,
            request.LastName,
            request.PhoneNumber);

        var result = await sender.Send(command, cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record UpdateProfileRequest(
    string FirstName,
    string LastName,
    string? PhoneNumber);
