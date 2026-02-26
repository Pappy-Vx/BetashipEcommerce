using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Orders.CancelOrder;

public sealed record CancelOrderCommand(
    Guid OrderId,
    string Reason) : IRequest<Result>;
