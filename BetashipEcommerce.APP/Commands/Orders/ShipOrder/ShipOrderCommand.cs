using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Orders.ShipOrder;

public sealed record ShipOrderCommand(
    Guid OrderId,
    string? TrackingNumber) : IRequest<Result>;
