using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Identity
{
    public static class Permissions
    {
        // Product permissions
        public const string ViewProducts = "products.view";
        public const string CreateProducts = "products.create";
        public const string UpdateProducts = "products.update";
        public const string DeleteProducts = "products.delete";
        public const string PublishProducts = "products.publish";

        // Inventory permissions
        public const string ViewInventory = "inventory.view";
        public const string ManageInventory = "inventory.manage";
        public const string AdjustStock = "inventory.adjust";

        // Order permissions
        public const string ViewOrders = "orders.view";
        public const string ViewAllOrders = "orders.viewall";
        public const string CreateOrders = "orders.create";
        public const string UpdateOrders = "orders.update";
        public const string CancelOrders = "orders.cancel";
        public const string ShipOrders = "orders.ship";

        // Customer permissions
        public const string ViewCustomers = "customers.view";
        public const string UpdateCustomers = "customers.update";
        public const string DeleteCustomers = "customers.delete";

        // User management permissions
        public const string ViewUsers = "users.view";
        public const string CreateUsers = "users.create";
        public const string UpdateUsers = "users.update";
        public const string DeleteUsers = "users.delete";
        public const string ManageRoles = "users.roles";

        // Payment permissions
        public const string ViewPayments = "payments.view";
        public const string ProcessPayments = "payments.process";
        public const string RefundPayments = "payments.refund";

        // Audit permissions
        public const string ViewAuditLogs = "audit.view";

        // System permissions
        public const string ManageSystem = "system.manage";

        public static List<string> CustomerPermissions => new()
    {
        ViewProducts,
        CreateOrders,
        ViewOrders
    };

        public static List<string> SupportPermissions => new()
    {
        ViewProducts,
        ViewCustomers,
        ViewAllOrders,
        ViewPayments
    };

        public static List<string> OrderPermissions => new()
    {
        ViewProducts,
        ViewAllOrders,
        UpdateOrders,
        CancelOrders,
        ShipOrders,
        ViewCustomers,
        ViewPayments
    };

        public static List<string> InventoryPermissions => new()
    {
        ViewProducts,
        CreateProducts,
        UpdateProducts,
        PublishProducts,
        ViewInventory,
        ManageInventory,
        AdjustStock
    };

        public static List<string> AdminPermissions => new()
    {
        ViewProducts, CreateProducts, UpdateProducts, DeleteProducts, PublishProducts,
        ViewInventory, ManageInventory, AdjustStock,
        ViewAllOrders, CreateOrders, UpdateOrders, CancelOrders, ShipOrders,
        ViewCustomers, UpdateCustomers, DeleteCustomers,
        ViewUsers, CreateUsers, UpdateUsers, DeleteUsers, ManageRoles,
        ViewPayments, ProcessPayments, RefundPayments,
        ViewAuditLogs
    };

        public static List<string> GetAllPermissions()
        {
            var allPerms = new List<string>(AdminPermissions) { ManageSystem };
            return allPerms;
        }
    }

}
