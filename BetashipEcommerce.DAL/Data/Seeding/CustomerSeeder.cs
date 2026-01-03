using BetashipEcommerce.CORE.Customers;
using BetashipEcommerce.CORE.Customers.Entities;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Data.Seeding
{
    internal sealed class CustomerSeeder
    {
        private readonly ApplicationDbContext _context;

        public CustomerSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            var customers = CreateCustomers();
            _context.Customers.AddRange(customers);
            await Task.CompletedTask;
        }

        private List<Customer> CreateCustomers()
        {
            var customers = new List<Customer>();

            // Customer 1 - John Doe
            var customer1 = Customer.Create(
                "john.doe@example.com",
                "John",
                "Doe"
            ).Value;
            customer1.UpdateProfile("John", "Doe", "+1234567890");

            var address1 = CustomerAddress.Create(
                "Home",
                Address.Create(
                    "123 Main Street",
                    "New York",
                    "NY",
                    "United States",
                    "10001"
                ),
                true
            );
            customer1.AddAddress(address1);
            customers.Add(customer1);

            // Customer 2 - Jane Smith
            var customer2 = Customer.Create(
                "jane.smith@example.com",
                "Jane",
                "Smith"
            ).Value;
            customer2.UpdateProfile("Jane", "Smith", "+1987654321");

            var address2Home = CustomerAddress.Create(
                "Home",
                Address.Create(
                    "456 Oak Avenue",
                    "Los Angeles",
                    "CA",
                    "United States",
                    "90001"
                ),
                true
            );
            customer2.AddAddress(address2Home);

            var address2Work = CustomerAddress.Create(
                "Office",
                Address.Create(
                    "789 Business Blvd",
                    "Los Angeles",
                    "CA",
                    "United States",
                    "90002"
                ),
                false
            );
            customer2.AddAddress(address2Work);
            customers.Add(customer2);

            // Customer 3 - Bob Johnson
            var customer3 = Customer.Create(
                "bob.johnson@example.com",
                "Bob",
                "Johnson"
            ).Value;

            var address3 = CustomerAddress.Create(
                "Home",
                Address.Create(
                    "321 Pine Road",
                    "Chicago",
                    "IL",
                    "United States",
                    "60601"
                ),
                true
            );
            customer3.AddAddress(address3);
            customers.Add(customer3);

            return customers;
        }
    }
}
