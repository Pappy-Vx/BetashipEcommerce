using BetashipEcommerce.CORE.Products;
using BetashipEcommerce.CORE.Products.Entities;
using BetashipEcommerce.CORE.Products.Enums;
using BetashipEcommerce.CORE.Products.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Data.Seeding
{
    internal sealed class ProductSeeder
    {
        private readonly ApplicationDbContext _context;

        public ProductSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            var products = CreateProducts();
            _context.Products.AddRange(products);
            await Task.CompletedTask;
        }

        private List<Product> CreateProducts()
        {
            var products = new List<Product>();

            // Electronics Category
            products.AddRange(CreateElectronicsProducts());

            // Clothing Category
            products.AddRange(CreateClothingProducts());

            // Books Category
            products.AddRange(CreateBooksProducts());

            // Home & Garden Category
            products.AddRange(CreateHomeGardenProducts());

            // Sports & Outdoors Category
            products.AddRange(CreateSportsProducts());

            return products;
        }

        private List<Product> CreateElectronicsProducts()
        {
            var products = new List<Product>();

            // 1. iPhone 15 Pro
            var iphone = Product.Create(
                "iPhone 15 Pro",
                "Latest Apple iPhone with A17 Pro chip, 6.1-inch Super Retina XDR display, Pro camera system",
                "IP15P-256-TBL",
                Money.Create(999.00m, "USD"),
                ProductCategory.Electronics
            ).Value;
            iphone.AddImage(ProductImage.Create(
                "https://example.com/images/iphone15pro.jpg",
                "iPhone 15 Pro - Titanium Blue",
                1,
                true
            ));
            iphone.Publish();
            products.Add(iphone);

            // 2. MacBook Air M3
            var macbook = Product.Create(
                "MacBook Air 13\" M3",
                "Powerful Apple M3 chip, 13.6-inch Liquid Retina display, up to 18 hours battery life",
                "MBA-M3-13-SG",
                Money.Create(1299.00m, "USD"),
                ProductCategory.Electronics
            ).Value;
            macbook.AddImage(ProductImage.Create(
                "https://example.com/images/macbookair.jpg",
                "MacBook Air M3 - Space Gray",
                1,
                true
            ));
            macbook.Publish();
            products.Add(macbook);

            // 3. Samsung Galaxy S24 Ultra
            var samsung = Product.Create(
                "Samsung Galaxy S24 Ultra",
                "6.8\" Dynamic AMOLED display, 200MP camera, S Pen included, Snapdragon 8 Gen 3",
                "SGS24U-512-BLK",
                Money.Create(1199.00m, "USD"),
                ProductCategory.Electronics
            ).Value;
            samsung.AddImage(ProductImage.Create(
                "https://example.com/images/galaxys24.jpg",
                "Galaxy S24 Ultra - Titanium Black",
                1,
                true
            ));
            samsung.Publish();
            products.Add(samsung);

            // 4. Sony WH-1000XM5 Headphones
            var sonyHeadphones = Product.Create(
                "Sony WH-1000XM5 Wireless Headphones",
                "Industry-leading noise canceling, exceptional sound quality, 30-hour battery life",
                "SONY-WH1000XM5-BLK",
                Money.Create(399.99m, "USD"),
                ProductCategory.Electronics
            ).Value;
            sonyHeadphones.AddImage(ProductImage.Create(
                "https://example.com/images/sony-headphones.jpg",
                "Sony WH-1000XM5",
                1,
                true
            ));
            sonyHeadphones.Publish();
            products.Add(sonyHeadphones);

            // 5. Apple Watch Series 9
            var appleWatch = Product.Create(
                "Apple Watch Series 9",
                "Advanced health and fitness features, bright always-on Retina display, GPS + Cellular",
                "AW9-45MM-MID",
                Money.Create(429.00m, "USD"),
                ProductCategory.Electronics
            ).Value;
            appleWatch.AddImage(ProductImage.Create(
                "https://example.com/images/applewatch9.jpg",
                "Apple Watch Series 9 - Midnight",
                1,
                true
            ));
            appleWatch.Publish();
            products.Add(appleWatch);

            return products;
        }

        private List<Product> CreateClothingProducts()
        {
            var products = new List<Product>();

            // 1. Classic Cotton T-Shirt
            var tshirt = Product.Create(
                "Premium Cotton T-Shirt",
                "100% organic cotton, comfortable fit, available in multiple colors",
                "TSH-COTTON-M-BLK",
                Money.Create(29.99m, "USD"),
                ProductCategory.Clothing
            ).Value;
            tshirt.AddVariant(ProductVariant.Create("Small - Black", "TSH-COTTON-S-BLK", Money.Create(29.99m, "USD"), 50));
            tshirt.AddVariant(ProductVariant.Create("Medium - Black", "TSH-COTTON-M-BLK", Money.Create(29.99m, "USD"), 100));
            tshirt.AddVariant(ProductVariant.Create("Large - Black", "TSH-COTTON-L-BLK", Money.Create(29.99m, "USD"), 75));
            tshirt.AddImage(ProductImage.Create(
                "https://example.com/images/tshirt-black.jpg",
                "Premium Cotton T-Shirt - Black",
                1,
                true
            ));
            tshirt.Publish();
            products.Add(tshirt);

            // 2. Denim Jeans
            var jeans = Product.Create(
                "Classic Fit Denim Jeans",
                "Premium denim, comfortable stretch, classic fit, durable construction",
                "JEANS-CF-32-BLU",
                Money.Create(79.99m, "USD"),
                ProductCategory.Clothing
            ).Value;
            jeans.AddVariant(ProductVariant.Create("30x32", "JEANS-CF-30-BLU", Money.Create(79.99m, "USD"), 40));
            jeans.AddVariant(ProductVariant.Create("32x32", "JEANS-CF-32-BLU", Money.Create(79.99m, "USD"), 60));
            jeans.AddVariant(ProductVariant.Create("34x32", "JEANS-CF-34-BLU", Money.Create(79.99m, "USD"), 50));
            jeans.AddImage(ProductImage.Create(
                "https://example.com/images/jeans-blue.jpg",
                "Classic Fit Denim Jeans",
                1,
                true
            ));
            jeans.Publish();
            products.Add(jeans);

            // 3. Winter Jacket
            var jacket = Product.Create(
                "Premium Winter Jacket",
                "Waterproof, insulated, windproof, perfect for cold weather",
                "JKT-WNTR-L-NVY",
                Money.Create(149.99m, "USD"),
                ProductCategory.Clothing
            ).Value;
            jacket.AddImage(ProductImage.Create(
                "https://example.com/images/winter-jacket.jpg",
                "Premium Winter Jacket - Navy",
                1,
                true
            ));
            jacket.Publish();
            products.Add(jacket);

            return products;
        }

        private List<Product> CreateBooksProducts()
        {
            var products = new List<Product>();

            // 1. Domain-Driven Design Book
            var dddBook = Product.Create(
                "Domain-Driven Design: Tackling Complexity in the Heart of Software",
                "Eric Evans' seminal work on domain-driven design principles and patterns",
                "BOOK-DDD-HC",
                Money.Create(54.99m, "USD"),
                ProductCategory.Books
            ).Value;
            dddBook.AddImage(ProductImage.Create(
                "https://example.com/images/ddd-book.jpg",
                "Domain-Driven Design Book",
                1,
                true
            ));
            dddBook.Publish();
            products.Add(dddBook);

            // 2. Clean Architecture Book
            var cleanArchBook = Product.Create(
                "Clean Architecture: A Craftsman's Guide to Software Structure",
                "Robert C. Martin's guide to creating maintainable software architectures",
                "BOOK-CLEAN-PB",
                Money.Create(39.99m, "USD"),
                ProductCategory.Books
            ).Value;
            cleanArchBook.AddImage(ProductImage.Create(
                "https://example.com/images/clean-arch-book.jpg",
                "Clean Architecture Book",
                1,
                true
            ));
            cleanArchBook.Publish();
            products.Add(cleanArchBook);

            // 3. Design Patterns Book
            var designPatternsBook = Product.Create(
                "Design Patterns: Elements of Reusable Object-Oriented Software",
                "The Gang of Four's classic book on software design patterns",
                "BOOK-DP-HC",
                Money.Create(59.99m, "USD"),
                ProductCategory.Books
            ).Value;
            designPatternsBook.AddImage(ProductImage.Create(
                "https://example.com/images/design-patterns-book.jpg",
                "Design Patterns Book",
                1,
                true
            ));
            designPatternsBook.Publish();
            products.Add(designPatternsBook);

            return products;
        }

        private List<Product> CreateHomeGardenProducts()
        {
            var products = new List<Product>();

            // 1. Coffee Maker
            var coffeeMaker = Product.Create(
                "Premium Programmable Coffee Maker",
                "12-cup capacity, programmable brew start, auto shut-off, permanent filter",
                "CM-PREM-12C-BLK",
                Money.Create(89.99m, "USD"),
                ProductCategory.HomeAndGarden
            ).Value;
            coffeeMaker.AddImage(ProductImage.Create(
                "https://example.com/images/coffee-maker.jpg",
                "Premium Coffee Maker",
                1,
                true
            ));
            coffeeMaker.Publish();
            products.Add(coffeeMaker);

            // 2. Robot Vacuum
            var robotVacuum = Product.Create(
                "Smart Robot Vacuum Cleaner",
                "AI-powered navigation, automatic charging, app control, 2000Pa suction",
                "RV-SMART-2000",
                Money.Create(299.99m, "USD"),
                ProductCategory.HomeAndGarden
            ).Value;
            robotVacuum.AddImage(ProductImage.Create(
                "https://example.com/images/robot-vacuum.jpg",
                "Smart Robot Vacuum",
                1,
                true
            ));
            robotVacuum.Publish();
            products.Add(robotVacuum);

            // 3. Garden Tool Set
            var gardenTools = Product.Create(
                "Professional Garden Tool Set - 10 Pieces",
                "Durable stainless steel tools, ergonomic handles, includes carrying bag",
                "GDN-TOOL-10PC",
                Money.Create(49.99m, "USD"),
                ProductCategory.HomeAndGarden
            ).Value;
            gardenTools.AddImage(ProductImage.Create(
                "https://example.com/images/garden-tools.jpg",
                "Garden Tool Set",
                1,
                true
            ));
            gardenTools.Publish();
            products.Add(gardenTools);

            return products;
        }

        private List<Product> CreateSportsProducts()
        {
            var products = new List<Product>();

            // 1. Yoga Mat
            var yogaMat = Product.Create(
                "Premium Non-Slip Yoga Mat",
                "6mm thick, eco-friendly TPE material, non-slip surface, includes carrying strap",
                "YOGA-MAT-6MM-PUR",
                Money.Create(34.99m, "USD"),
                ProductCategory.SportsAndOutdoors
            ).Value;
            yogaMat.AddImage(ProductImage.Create(
                "https://example.com/images/yoga-mat.jpg",
                "Premium Yoga Mat - Purple",
                1,
                true
            ));
            yogaMat.Publish();
            products.Add(yogaMat);

            // 2. Camping Tent
            var tent = Product.Create(
                "4-Person Camping Tent",
                "Waterproof, easy setup, ventilation system, includes carrying bag",
                "TENT-4P-GRN",
                Money.Create(159.99m, "USD"),
                ProductCategory.SportsAndOutdoors
            ).Value;
            tent.AddImage(ProductImage.Create(
                "https://example.com/images/camping-tent.jpg",
                "4-Person Camping Tent",
                1,
                true
            ));
            tent.Publish();
            products.Add(tent);

            // 3. Adjustable Dumbbells
            var dumbbells = Product.Create(
                "Adjustable Dumbbells Set - 5-52.5 lbs",
                "Space-saving design, quick weight adjustment, includes stand",
                "DMBL-ADJ-52-SET",
                Money.Create(299.99m, "USD"),
                ProductCategory.SportsAndOutdoors
            ).Value;
            dumbbells.AddImage(ProductImage.Create(
                "https://example.com/images/dumbbells.jpg",
                "Adjustable Dumbbells Set",
                1,
                true
            ));
            dumbbells.Publish();
            products.Add(dumbbells);

            // 4. Mountain Bike
            var bike = Product.Create(
                "Mountain Bike 29\" - 21 Speed",
                "Aluminum frame, front suspension, disc brakes, 29-inch wheels",
                "MTB-29-21SPD-BLU",
                Money.Create(449.99m, "USD"),
                ProductCategory.SportsAndOutdoors
            ).Value;
            bike.AddImage(ProductImage.Create(
                "https://example.com/images/mountain-bike.jpg",
                "Mountain Bike 29\"",
                1,
                true
            ));
            bike.Publish();
            products.Add(bike);

            return products;
        }
    }

}
