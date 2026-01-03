using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Data.Seeding
{
    /// <summary>
    /// Main database seeder that orchestrates all seed operations
    /// </summary>
    public sealed class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IPasswordHasher _passwordHasher;

        public DatabaseSeeder(
            ApplicationDbContext context,
            ILogger<DatabaseSeeder> logger,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Seeds all initial data
        /// Safe to run multiple times - checks for existing data
        /// </summary>
        public async Task SeedAsync()
        {
            _logger.LogInformation("Starting database seeding...");

            try
            {
                // Ensure database is created
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Database migrations applied");

                // Seed in order (respecting dependencies)
                await SeedUsersAsync();
                await SeedCustomersAsync();
                await SeedProductsAsync();
                await SeedInventoryAsync();

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding database");
                throw;
            }
        }

        private async Task SeedUsersAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Users already seeded, skipping...");
                return;
            }

            _logger.LogInformation("Seeding users...");

            var userSeeder = new UserSeeder(_context, _passwordHasher);
            await userSeeder.SeedAsync();

            await _context.SaveChangesAsync();
            _logger.LogInformation("Users seeded successfully");
        }

        private async Task SeedCustomersAsync()
        {
            if (await _context.Customers.AnyAsync())
            {
                _logger.LogInformation("Customers already seeded, skipping...");
                return;
            }

            _logger.LogInformation("Seeding customers...");

            var customerSeeder = new CustomerSeeder(_context);
            await customerSeeder.SeedAsync();

            await _context.SaveChangesAsync();
            _logger.LogInformation("Customers seeded successfully");
        }

        private async Task SeedProductsAsync()
        {
            if (await _context.Products.AnyAsync())
            {
                _logger.LogInformation("Products already seeded, skipping...");
                return;
            }

            _logger.LogInformation("Seeding products...");

            var productSeeder = new ProductSeeder(_context);
            await productSeeder.SeedAsync();

            await _context.SaveChangesAsync();
            _logger.LogInformation("Products seeded successfully");
        }

        private async Task SeedInventoryAsync()
        {
            if (await _context.InventoryItems.AnyAsync())
            {
                _logger.LogInformation("Inventory already seeded, skipping...");
                return;
            }

            _logger.LogInformation("Seeding inventory...");

            var inventorySeeder = new InventorySeeder(_context);
            await inventorySeeder.SeedAsync();

            await _context.SaveChangesAsync();
            _logger.LogInformation("Inventory seeded successfully");
        }
    }

}
