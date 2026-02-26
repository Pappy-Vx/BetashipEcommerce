using BetashipEcommerce.APP.Common.Models;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Products.SearchProducts;

public sealed record SearchProductsQuery(
    string SearchTerm,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PaginatedList<ProductSearchResultDto>>;
