using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Domain;

public class PricingService
{
    private readonly Catalog _catalog;

    public PricingService(Catalog catalog)
    {
        _catalog = catalog;
    }

    public decimal CalculateTotal(Order order)
    {
        decimal total = 0m;
        foreach (var item in order.Items)
        {
            var product = _catalog.Products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null)
            {
                total += product.Price * item.Quantity;
            }
        }
        return total;
    }
}

public class OrderService
{
    private readonly IOrderRepository _orders;
    private readonly ICustomerRepository _customers;
    private readonly PricingService _pricing;

    public OrderService(IOrderRepository orders, ICustomerRepository customers, PricingService pricing)
    {
        _orders = orders;
        _customers = customers;
        _pricing = pricing;
    }

    public Order? Get(Guid id) => _orders.Get(id);
    public IEnumerable<Order> GetAll() => _orders.GetAll();

    public Order Create(CreateOrderRequest req)
    {
        var customer = _customers.Get(req.CustomerId);
        if (customer == null) throw new InvalidOperationException("Customer not found");

        var order = new Order
        {
            CustomerId = customer.Id,
            Items = req.Items.ToList(),
            Status = "Received",
        };

        _orders.Add(order);
        return order;
    }

    public async Task<IEnumerable<string>> ProcessOrderAsync(Guid id, CancellationToken ct = default)
    {
        var order = _orders.Get(id) ?? throw new InvalidOperationException("Order not found");
        var events = new List<string>();

        order.Status = "Brewing"; _orders.Update(order);
        events.Add("Brewing");
        await Task.Delay(200, ct);

        order.Status = "Packing"; _orders.Update(order);
        events.Add("Packing");
        await Task.Delay(200, ct);

        order.Status = "Ready"; _orders.Update(order);
        events.Add("Ready");
        await Task.Delay(200, ct);

        return events;
    }
}
