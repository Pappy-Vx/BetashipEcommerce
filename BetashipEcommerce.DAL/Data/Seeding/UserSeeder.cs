using BetashipEcommerce.CORE.Identity;
using BetashipEcommerce.CORE.Identity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Data.Seeding
{
    internal sealed class UserSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserSeeder(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            var users = CreateUsers();
            _context.Users.AddRange(users);
            await Task.CompletedTask;
        }

        private List<User> CreateUsers()
        {
            var users = new List<User>();

            // 1. Super Admin
            var superAdmin = User.Create(
                "superadmin",
                "superadmin@ecommerce.com",
                _passwordHasher.HashPassword("SuperAdmin@123"),
                "Super",
                "Admin"
            ).Value;
            superAdmin.AddRole(UserRole.SuperAdmin);
            superAdmin.VerifyEmail();
            users.Add(superAdmin);

            // 2. Admin
            var admin = User.Create(
                "admin",
                "admin@ecommerce.com",
                _passwordHasher.HashPassword("Admin@123"),
                "Admin",
                "User"
            ).Value;
            admin.AddRole(UserRole.Admin);
            admin.VerifyEmail();
            users.Add(admin);

            // 3. Inventory Manager
            var inventoryManager = User.Create(
                "inventory.manager",
                "inventory@ecommerce.com",
                _passwordHasher.HashPassword("Inventory@123"),
                "Inventory",
                "Manager"
            ).Value;
            inventoryManager.AddRole(UserRole.InventoryManager);
            inventoryManager.VerifyEmail();
            users.Add(inventoryManager);

            // 4. Order Manager
            var orderManager = User.Create(
                "order.manager",
                "orders@ecommerce.com",
                _passwordHasher.HashPassword("Orders@123"),
                "Order",
                "Manager"
            ).Value;
            orderManager.AddRole(UserRole.OrderManager);
            orderManager.VerifyEmail();
            users.Add(orderManager);

            // 5. Customer Support
            var support = User.Create(
                "support",
                "support@ecommerce.com",
                _passwordHasher.HashPassword("Support@123"),
                "Customer",
                "Support"
            ).Value;
            support.AddRole(UserRole.CustomerSupport);
            support.VerifyEmail();
            users.Add(support);

            // 6-8. Sample Customers
            var customer1 = User.Create(
                "john.doe",
                "john.doe@example.com",
                _passwordHasher.HashPassword("Customer@123"),
                "John",
                "Doe"
            ).Value;
            customer1.AddRole(UserRole.Customer);
            customer1.VerifyEmail();
            users.Add(customer1);

            var customer2 = User.Create(
                "jane.smith",
                "jane.smith@example.com",
                _passwordHasher.HashPassword("Customer@123"),
                "Jane",
                "Smith"
            ).Value;
            customer2.AddRole(UserRole.Customer);
            customer2.VerifyEmail();
            users.Add(customer2);

            var customer3 = User.Create(
                "bob.johnson",
                "bob.johnson@example.com",
                _passwordHasher.HashPassword("Customer@123"),
                "Bob",
                "Johnson"
            ).Value;
            customer3.AddRole(UserRole.Customer);
            users.Add(customer3); // Not verified

            return users;
        }
    }
}
