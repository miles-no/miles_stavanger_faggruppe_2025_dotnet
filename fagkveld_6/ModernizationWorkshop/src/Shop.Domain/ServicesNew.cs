using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Domain;

// Modernized services using primary constructors, Lazy<T>, async streams, and yield.

public class PricingServiceNew(Catalog catalog)
{
    private readonly Catalog _catalog = catalog;

    public Money CalculateTotal(OrderNew order)
    {
        decimal total = 0m;
        foreach (var item in order.Items)
        {
            var product = _catalog.Products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product is not null)
            {
                total += product.Price * item.Quantity;
            }
        }
        return new Money(total);
    }
}

public class OrderServiceNew(IOrderRepository orders, ICustomerRepository customers, Lazy<PricingServiceNew> pricing)
{
    private readonly IOrderRepository _orders = orders;
    private readonly ICustomerRepository _customers = customers;
    private readonly Lazy<PricingServiceNew> _pricing = pricing;

    public OrderNew? Get(Guid id)
    {
        var old = _orders.Get(id);
        if (old is null) return null;
        var items = old.Items.Select(i => new OrderItemNew(i.ProductId, i.Quantity)).ToList();
        return new OrderNew(old.Id, old.CustomerId, old.CreatedUtc, items, old.Status);
    }

    public IEnumerable<OrderNew> GetAll()
        => _orders.GetAll().Select(o => new OrderNew(
            o.Id, o.CustomerId, o.CreatedUtc,
            o.Items.Select(i => new OrderItemNew(i.ProductId, i.Quantity)).ToList(),
            o.Status));

    public OrderNew Create(CreateOrderRequestNew req)
    {
        var customer = _customers.Get(req.CustomerId) ?? throw new InvalidOperationException("Customer not found");
        var items = req.Items.ToList();
        var orderNew = OrderNew.New(customer.Id, items);
        // save as old model to reuse repository
        var old = new Order { Id = orderNew.Id, CustomerId = orderNew.CustomerId, CreatedUtc = orderNew.CreatedUtc, Items = items.Select(i => new OrderItem { ProductId = i.ProductId, Quantity = i.Quantity }).ToList(), Status = orderNew.Status };
        _orders.Add(old);
        return orderNew;
    }

    // Async stream: emits order status over time using yield return
    public async IAsyncEnumerable<string> ProcessOrderStreamAsync(Guid id, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var order = _orders.Get(id) ?? throw new InvalidOperationException("Order not found");

        order.Status = "Brewing"; _orders.Update(order);
        yield return "Brewing";
        await Task.Delay(200, ct);

        order.Status = "Packing"; _orders.Update(order);
        yield return "Packing";
        await Task.Delay(200, ct);

        order.Status = "Ready"; _orders.Update(order);
        yield return "Ready";
        await Task.Delay(200, ct);
    }
}
