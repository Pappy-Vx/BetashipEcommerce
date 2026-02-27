using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetashipEcommerce.CORE.SharedKernel;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BetashipEcommerce.CORE.Products
{
    public static class ProductErrors
    {
        public static readonly Error InvalidName = new("Product.InvalidName",
            "Product name cannot be empty");

        public static readonly Error InvalidSku = new("Product.InvalidSku",
            "Product SKU cannot be empty");

        public static readonly Error InvalidPrice = new("Product.InvalidPrice",
            "Product price must be greater than zero");

        public static readonly Error InvalidStock = new("Product.InvalidStock",
            "Stock quantity cannot be negative");

        public static readonly Error InsufficientStock = new("Product.InsufficientStock",
            "Insufficient stock for this operation");

        public static readonly Error AlreadyPublished = new("Product.AlreadyPublished",
            "Product is already published");

        public static readonly Error AlreadyDiscontinued = new("Product.AlreadyDiscontinued",
        "Product is already discontinued");

        public static readonly Error CannotPublishWithoutStock = new("Product.CannotPublishWithoutStock",
            "Cannot publish product without available stock");

        public static readonly Error NotFound = new("Product.NotFound",
            "Product not found");

        public static readonly Error DuplicateSku = new("Product.DuplicateSku",
            "A product with this SKU already exists");
    }
}
