using BetashipEcommerce.CORE.Carts.ValueObjects;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Carts.Events
{
    public sealed record ShoppingCartCreatedDomainEvent(
    ShoppingCartId CartId,
    CustomerId CustomerId) : DomainEvent;

    public sealed record ItemAddedToCartDomainEvent(
        ShoppingCartId CartId,
        CustomerId CustomerId,
        ProductId ProductId,
        int Quantity) : DomainEvent;

    public sealed record CartItemQuantityUpdatedDomainEvent(
        ShoppingCartId CartId,
        ProductId ProductId,
        int NewQuantity) : DomainEvent;

    public sealed record ItemRemovedFromCartDomainEvent(
        ShoppingCartId CartId,
        ProductId ProductId) : DomainEvent;

    public sealed record ShoppingCartClearedDomainEvent(
        ShoppingCartId CartId) : DomainEvent;

    public sealed record ShoppingCartConvertedToOrderDomainEvent(
        ShoppingCartId CartId,
        CustomerId CustomerId) : DomainEvent;

    public sealed record ShoppingCartAbandonedDomainEvent(
        ShoppingCartId CartId,
        CustomerId CustomerId,
        int UniqueItemsCount,
        int TotalItemsCount) : DomainEvent;

}
