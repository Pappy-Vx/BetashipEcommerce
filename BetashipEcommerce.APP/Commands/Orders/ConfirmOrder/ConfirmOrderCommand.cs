using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Orders.ConfirmOrder;

public sealed record ConfirmOrderCommand(Guid OrderId) : IRequest<Result>;
