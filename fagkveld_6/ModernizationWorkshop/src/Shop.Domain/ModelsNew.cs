using System;
using System.Collections.Generic;

namespace Shop.Domain;

// Modernized models using records and collection expressions.
// TODO(Workshop): Convert old classes to records with init-only members where needed.
// Include with-expressions and deconstruction examples in tests/README.

public readonly record struct Money(decimal Amount);

public record CustomerNew(Guid Id, string Name, string Email);

public record ProductNew(Guid Id, string Sku, string Name, Money Price);

public record OrderItemNew(Guid ProductId, int Quantity);

public record OrderNew(Guid Id, Guid CustomerId, DateTime CreatedUtc, IReadOnlyList<OrderItemNew> Items, string Status)
{
    public static OrderNew New(Guid customerId, IReadOnlyList<OrderItemNew> items)
        => new(Guid.NewGuid(), customerId, DateTime.UtcNow, items, "Received");
}

public record CreateOrderRequestNew(Guid CustomerId, IReadOnlyList<OrderItemNew> Items);
