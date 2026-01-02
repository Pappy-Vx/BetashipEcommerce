using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Identity.Enums
{
    public enum UserRole
    {
        Customer = 1,           // Regular customer
        CustomerSupport = 2,    // Can view orders, customers
        OrderManager = 3,       // Can manage orders
        InventoryManager = 4,   // Can manage products, inventory
        Admin = 5,              // Can manage everything except system settings
        SuperAdmin = 6          // Full system access
    }
}
