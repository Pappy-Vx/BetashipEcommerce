using MediatR;

namespace BetashipEcommerce.APP.Common.Models;

/// <summary>
/// Base class for paged queries to avoid duplication across query types.
/// </summary>
public abstract record PagedQuery<TResponse> : IRequest<TResponse>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
