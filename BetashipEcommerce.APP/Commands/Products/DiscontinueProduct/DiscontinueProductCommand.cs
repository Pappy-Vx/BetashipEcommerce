using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Products.DiscontinueProduct;

public sealed record DiscontinueProductCommand(
    Guid ProductId,
    string Reason) : IRequest<Result>;
