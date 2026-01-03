using BetashipEcommerce.CORE.Inventory;
using BetashipEcommerce.CORE.Products;
using BetashipEcommerce.CORE.Products.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Data.Seeding
{
    internal sealed class InventorySeeder
    {
        private readonly ApplicationDbContext _context;

        public InventorySeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Get all products that need inventory
            var products = await _context.Products
                .Where(p => p.Status == ProductStatus.Published)
                .ToListAsync();

            var inventoryItems = new List<InventoryItem>();

            foreach (var product in products)
            {
                var inventoryItem = CreateInventoryForProduct(product);
                inventoryItems.Add(inventoryItem);
            }

            _context.InventoryItems.AddRange(inventoryItems);
        }

        private InventoryItem CreateInventoryForProduct(Product product)
        {
            // Determine initial stock based on product category
            int initialStock = product.Category switch
            {
                ProductCategory.Electronics => 50,
                ProductCategory.Clothing => 100,
                ProductCategory.Books => 200,
                ProductCategory.HomeAndGarden => 75,
                ProductCategory.SportsAndOutdoors => 60,
                _ => 50
            };

            // Determine reorder levels
            int reorderLevel = product.Category switch
            {
                ProductCategory.Electronics => 10,
                ProductCategory.Clothing => 20,
                ProductCategory.Books => 30,
                ProductCategory.HomeAndGarden => 15,
                ProductCategory.SportsAndOutdoors => 12,
                _ => 10
            };

            int reorderQuantity = reorderLevel * 3;

            return InventoryItem.Create(
                product.Id,
                initialStock,
                reorderLevel,
                reorderQuantity
            ).Value;
        }
    }
}
