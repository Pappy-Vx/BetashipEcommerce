using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetashipEcommerce.CORE.SharedKernel;

namespace BetashipEcommerce.CORE.Payments
{
    public static class PaymentErrors
    {
        public static readonly Error InvalidAmount = new("Payment.InvalidAmount",
            "Payment amount must be greater than zero");

        public static readonly Error InvalidStatusTransition = new("Payment.InvalidStatusTransition",
            "Invalid payment status transition");

        public static readonly Error InvalidTransactionId = new("Payment.InvalidTransactionId",
            "Transaction ID cannot be empty");

        public static readonly Error CannotFailCompletedPayment = new("Payment.CannotFailCompletedPayment",
            "Cannot fail a completed or refunded payment");

        public static readonly Error CanOnlyRetryFailedPayments = new("Payment.CanOnlyRetryFailedPayments",
            "Can only retry failed payments");

        public static readonly Error MaxRetryAttemptsExceeded = new("Payment.MaxRetryAttemptsExceeded",
            "Maximum retry attempts exceeded");

        public static readonly Error CanOnlyRefundCompletedPayments = new("Payment.CanOnlyRefundCompletedPayments",
            "Can only refund completed payments");

        public static readonly Error InvalidRefundAmount = new("Payment.InvalidRefundAmount",
            "Invalid refund amount");

        public static readonly Error CurrencyMismatch = new("Payment.CurrencyMismatch",
            "Refund currency must match payment currency");

        public static readonly Error CannotCancelCompletedPayment = new("Payment.CannotCancelCompletedPayment",
            "Cannot cancel a completed or refunded payment");

        public static readonly Error NotFound = new("Payment.NotFound",
            "Payment not found");
    }

}
