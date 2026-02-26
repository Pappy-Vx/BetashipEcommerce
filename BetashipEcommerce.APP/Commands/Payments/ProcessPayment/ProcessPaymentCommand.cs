using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Payments.ProcessPayment;

public sealed record ProcessPaymentCommand(
    Guid PaymentId,
    string TransactionId,
    string? GatewayResponse) : IRequest<Result>;
