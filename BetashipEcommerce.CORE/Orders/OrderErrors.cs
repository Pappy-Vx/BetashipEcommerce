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

        public static readonly Error InvalidStatusTransition = new("Order.InvalidStatusTransition",
            "Invalid order status transition");

        public static readonly Error InventoryAlreadyReserved = new("Order.InventoryAlreadyReserved",
            "Inventory has already been reserved for this order");

        public static readonly Error InventoryNotReserved = new("Order.InventoryNotReserved",
            "Inventory must be reserved before proceeding");

        public static readonly Error OrderNotPaid = new("Order.OrderNotPaid",
            "Order must be paid before proceeding");

        public static readonly Error OrderAlreadyPaid = new("Order.OrderAlreadyPaid",
            "Order has already been paid");

        public static readonly Error InventoryNotCommitted = new("Order.InventoryNotCommitted",
            "Inventory must be committed before confirming order");

        public static readonly Error InventoryAlreadyCommitted = new("Order.InventoryAlreadyCommitted",
            "Inventory has already been committed");

        public static readonly Error CannotCancelDeliveredOrder = new("Order.CannotCancelDeliveredOrder",
            "Cannot cancel an order that has been delivered");

        public static readonly Error CannotCancelShippedOrder = new("Order.CannotCancelShippedOrder",
            "Cannot cancel an order that has been shipped");

        public static readonly Error OrderAlreadyCancelled = new("Order.OrderAlreadyCancelled",
            "Order has already been cancelled");

        public static readonly Error NotFound = new("Order.NotFound",
            "Order not found");
    }

}
