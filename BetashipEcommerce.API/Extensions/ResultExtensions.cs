using BetashipEcommerce.CORE.SharedKernel;

namespace BetashipEcommerce.API.Extensions;

public static class ResultExtensions
{
    public static IResult ToApiResult(this Result result)
    {
        if (result.IsSuccess)
            return Results.NoContent();

        return MapErrorToResult(result.Error);
    }

    public static IResult ToApiResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result.Value);

        return MapErrorToResult(result.Error);
    }

    public static IResult ToCreatedResult<T>(this Result<T> result, string routeName, object routeValues)
    {
        if (result.IsSuccess)
            return Results.CreatedAtRoute(routeName, routeValues, result.Value);

        return MapErrorToResult(result.Error);
    }

    private static IResult MapErrorToResult(Error error)
    {
        var code = error.Code;

        if (code.EndsWith(".NotFound", StringComparison.OrdinalIgnoreCase))
            return Results.NotFound(new ProblemDetail(error.Code, error.Message, 404));

        if (code.Contains("Already", StringComparison.OrdinalIgnoreCase) ||
            code.Contains("Duplicate", StringComparison.OrdinalIgnoreCase) ||
            code.Contains("Conflict", StringComparison.OrdinalIgnoreCase))
            return Results.Conflict(new ProblemDetail(error.Code, error.Message, 409));

        if (code.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase))
            return Results.Unauthorized();

        if (code.Contains("Forbidden", StringComparison.OrdinalIgnoreCase))
            return Results.Forbid();

        return Results.BadRequest(new ProblemDetail(error.Code, error.Message, 400));
    }

    private sealed record ProblemDetail(string Code, string Message, int StatusCode);
}
