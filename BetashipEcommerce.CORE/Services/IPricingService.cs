using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Services
{
    public interface IPricingService : IDomainService
    {
        Result<Money> CalculateDiscount(Money originalPrice, decimal discountPercentage);
        Result<Money> ApplyTax(Money amount, decimal taxRate);
    }
}
