using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Payments.RefundPayment;

public sealed record RefundPaymentCommand(
    Guid PaymentId,
    decimal RefundAmount,
    string Currency,
    string Reason) : IRequest<Result>;
