using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Payments.InitiatePayment;

public sealed record InitiatePaymentCommand(
    Guid OrderId,
    Guid CustomerId,
    decimal Amount,
    string Currency,
    int PaymentMethod) : IRequest<Result<Guid>>;
