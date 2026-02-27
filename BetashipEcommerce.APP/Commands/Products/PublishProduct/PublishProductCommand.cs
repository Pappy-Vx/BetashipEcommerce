using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Products.PublishProduct;

public sealed record PublishProductCommand(Guid ProductId) : IRequest<Result>;
