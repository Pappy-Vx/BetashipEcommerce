using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetashipEcommerce.CORE.SharedKernel;

namespace BetashipEcommerce.CORE.Carts
{
    public static class CartErrors
    {
        public static readonly Error InvalidQuantity = new("Cart.InvalidQuantity",
            "Quantity must be greater than zero");

        public static readonly Error ItemNotFound = new("Cart.ItemNotFound",
            "Item not found in cart");

        public static readonly Error EmptyCart = new("Cart.EmptyCart",
            "Cart is empty");

        public static readonly Error CartNotActive = new("Cart.CartNotActive",
            "Cart is not active");

        public static readonly Error CartExpired = new("Cart.CartExpired",
            "Cart has expired");

        public static readonly Error CartAlreadyCheckedOut = new("Cart.CartAlreadyCheckedOut",
            "Cart has already been checked out");

        public static readonly Error NotFound = new("Cart.NotFound",
            "Shopping cart not found");
    }

}
