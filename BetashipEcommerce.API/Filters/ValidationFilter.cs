using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BetashipEcommerce.API.Filters;

/// <summary>
/// Endpoint filter that runs FluentValidation on the request body before reaching the handler.
/// Attach with .AddEndpointFilter&lt;ValidationFilter&lt;T&gt;&gt;() on an endpoint.
/// </summary>
public sealed class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var argument = context.Arguments.OfType<T>().FirstOrDefault();

        if (argument is null)
            return Results.BadRequest(new ProblemDetails
            {
                Title  = "Bad Request",
                Detail = $"Could not bind request to {typeof(T).Name}.",
                Status = StatusCodes.Status400BadRequest
            });

        var validationResult = await validator.ValidateAsync(argument, context.HttpContext.RequestAborted);

        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        return await next(context);
    }
}
