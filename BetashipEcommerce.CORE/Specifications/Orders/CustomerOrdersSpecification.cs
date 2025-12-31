using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Specifications.Orders
{
    public sealed class CustomerOrdersSpecification : Specification<Order>
    {
        public CustomerOrdersSpecification(CustomerId customerId)
            : base(o => o.CustomerId == customerId)
        {
            ApplyOrderByDescending(o => o.OrderDate);
            AddInclude(o => o.Items);
        }
    }
}
