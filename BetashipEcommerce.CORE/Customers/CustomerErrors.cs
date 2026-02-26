using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetashipEcommerce.CORE.SharedKernel;

namespace BetashipEcommerce.CORE.Customers
{
    public static class CustomerErrors
    {
        public static readonly Error InvalidEmail = new("Customer.InvalidEmail",
            "Invalid email address format");

        public static readonly Error InvalidFirstName = new("Customer.InvalidFirstName",
            "First name cannot be empty");

        public static readonly Error InvalidLastName = new("Customer.InvalidLastName",
            "Last name cannot be empty");

        public static readonly Error InvalidPhoneNumber = new("Customer.InvalidPhoneNumber",
            "Invalid phone number format");

        public static readonly Error AddressNotFound = new("Customer.AddressNotFound",
            "Address not found");

        public static readonly Error AlreadyInactive = new("Customer.AlreadyInactive",
            "Customer is already inactive");

        public static readonly Error AlreadyActive = new("Customer.AlreadyActive",
            "Customer is already active");

        public static readonly Error NotFound = new("Customer.NotFound",
            "Customer not found");

        public static readonly Error DuplicateEmail = new("Customer.DuplicateEmail",
            "A customer with this email already exists");
    }
}
