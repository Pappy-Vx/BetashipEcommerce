using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetashipEcommerce.CORE.SharedKernel;

namespace BetashipEcommerce.CORE.Inventory
{
    public static class InventoryErrors
    {
        public static readonly Error InvalidQuantity = new("Inventory.InvalidQuantity",
            "Quantity must be greater than zero");

        public static readonly Error InsufficientStock = new("Inventory.InsufficientStock",
            "Insufficient stock available");

        public static readonly Error InvalidReorderLevel = new("Inventory.InvalidReorderLevel",
            "Reorder level cannot be negative");

        public static readonly Error InvalidReorderQuantity = new("Inventory.InvalidReorderQuantity",
            "Reorder quantity must be greater than zero");

        public static readonly Error ReservationNotFound = new("Inventory.ReservationNotFound",
            "Stock reservation not found");

        public static readonly Error ReservationExpired = new("Inventory.ReservationExpired",
            "Stock reservation has expired");

        public static readonly Error InvalidReservationStatus = new("Inventory.InvalidReservationStatus",
            "Invalid reservation status for this operation");

        public static readonly Error CannotReleaseCommittedReservation = new("Inventory.CannotReleaseCommittedReservation",
            "Cannot release a committed reservation");

        public static readonly Error NotFound = new("Inventory.NotFound",
            "Inventory item not found");
    }
}
