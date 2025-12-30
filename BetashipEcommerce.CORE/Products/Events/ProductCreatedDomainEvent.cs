using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Products.Events
{
    public sealed record ProductCreatedDomainEvent(
     ProductId ProductId,
     string ProductName) : DomainEvent;

    public sealed record ProductPriceChangedDomainEvent(
        ProductId ProductId,
        decimal OldPrice,
        decimal NewPrice,
        string Currency) : DomainEvent;

    public sealed record ProductStockAdjustedDomainEvent(
        ProductId ProductId,
        int OldQuantity,
        int NewQuantity,
        string Reason) : DomainEvent;

    public sealed record ProductPublishedDomainEvent(
        ProductId ProductId,
        string ProductName) : DomainEvent;
}
