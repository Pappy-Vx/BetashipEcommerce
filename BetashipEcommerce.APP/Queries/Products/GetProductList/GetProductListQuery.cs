using BetashipEcommerce.APP.Common.Models;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Products.GetProductList;

public sealed record GetProductListQuery : IRequest<PaginatedList<ProductListDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? CategoryFilter { get; init; }
    public string? StatusFilter { get; init; }
    public string? SearchTerm { get; init; }
}
