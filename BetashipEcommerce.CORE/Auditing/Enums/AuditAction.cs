using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Auditing.Enums
{
    public enum AuditAction
    {
        // General CRUD
        Created = 1,
        Updated = 2,
        Deleted = 3,
        Viewed = 4,

        // Product specific
        ProductPublished = 100,
        ProductDiscontinued = 101,
        PriceChanged = 102,

        // Inventory specific
        StockAdded = 200,
        StockRemoved = 201,
        StockReserved = 202,
        StockCommitted = 203,
        StockReleased = 204,

        // Order specific
        OrderPlaced = 300,
        OrderConfirmed = 301,
        OrderShipped = 302,
        OrderDelivered = 303,
        OrderCancelled = 304,

        // Payment specific
        PaymentInitiated = 400,
        PaymentCompleted = 401,
        PaymentFailed = 402,
        PaymentRefunded = 403,

        // User/Auth specific
        UserLoggedIn = 500,
        UserLoggedOut = 501,
        UserPasswordChanged = 502,
        UserRoleChanged = 503,
        UserLocked = 504,
        UserUnlocked = 505,

        // System
        SystemConfigChanged = 900
    }

}
