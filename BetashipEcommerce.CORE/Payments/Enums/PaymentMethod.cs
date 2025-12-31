using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Payments.Enums
{
    public enum PaymentMethod
    {
        CreditCard = 1,
        DebitCard = 2,
        PayPal = 3,
        BankTransfer = 4,
        CashOnDelivery = 5,
        Cryptocurrency = 6
    }
}
