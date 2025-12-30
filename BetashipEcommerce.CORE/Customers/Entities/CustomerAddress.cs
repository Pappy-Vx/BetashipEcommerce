using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Customers.Entities
{
    public sealed class CustomerAddress : Entity<Guid>
    {
        public string Label { get; private set; } // e.g., "Home", "Office"
        public Address Address { get; private set; }
        public bool IsDefault { get; private set; }

        private CustomerAddress(Guid id, string label, Address address, bool isDefault)
            : base(id)
        {
            Label = label;
            Address = address;
            IsDefault = isDefault;
        }

        private CustomerAddress() : base() { }

        public static CustomerAddress Create(string label, Address address, bool isDefault = false)
        {
            return new CustomerAddress(Guid.NewGuid(), label, address, isDefault);
        }

        public void SetAsDefault(bool isDefault)
        {
            IsDefault = isDefault;
        }
    }

}
