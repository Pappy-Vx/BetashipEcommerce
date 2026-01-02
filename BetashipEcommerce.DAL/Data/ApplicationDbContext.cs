using BetashipEcommerce.CORE.Auditing;
using BetashipEcommerce.CORE.Carts;
using BetashipEcommerce.CORE.Customers;
using BetashipEcommerce.CORE.Identity;
using BetashipEcommerce.CORE.Inventory;
using BetashipEcommerce.CORE.Orders;
using BetashipEcommerce.CORE.Payments;
using BetashipEcommerce.CORE.Products;
using BetashipEcommerce.DAL.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Data
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Aggregates
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
        public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<User> Users => Set<User>();

        // Audit & Outbox
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public DbSet<OutboxMessageConsumer> OutboxMessageConsumers => Set<OutboxMessageConsumer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Global query filters for soft delete
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Customer>().HasQueryFilter(c => !EF.Property<bool>(c, "IsDeleted"));
            modelBuilder.Entity<User>().HasQueryFilter(u => !EF.Property<bool>(u, "IsDeleted"));

            // Configure schema
            modelBuilder.HasDefaultSchema("ecommerce");
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Interceptors will handle audit fields and outbox messages
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
