using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetashipEcommerce.CORE.SharedKernel;

namespace BetashipEcommerce.CORE.Orders
{
    public static class OrderErrors
    {
        public static readonly Error EmptyOrder = new("Order.EmptyOrder",
            "Order must contain at least one item");

        public static readonly Error InvalidQuantity = new("Order.InvalidQuantity",
            "Quantity must be greater than zero");

        public static readonly Error CannotModifyConfirmedOrder = new("Order.CannotModifyConfirmedOrder",
            "Cannot modify an order that has been confirmed");

        public static readonly Error ItemNotFound = new("Order.ItemNotFound",
            "Order item not found");

        public static readonly Error InvalidStatusTransition = new("Order.InvalidStatusTransition",
            "Invalid order status transition");

        public static readonly Error CannotCancelOrder = new("Order.CannotCancelOrder",
            "Order cannot be cancelled in its current status");

        public static readonly Error NotFound = new("Order.NotFound",
            "Order not found");
    }

}
