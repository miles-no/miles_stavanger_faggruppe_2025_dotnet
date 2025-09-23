using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Domain;

// Old-style services (will create ServicesNew.cs variants with primary constructors, async streams, Lazy<T>, yield, etc.)

public interface ICustomerRepository
{
    Customer? Get(Guid id);
    IEnumerable<Customer> GetAll();
    void Add(Customer c);
}

public interface IProductRepository
{
    Product? Get(Guid id);
    IEnumerable<Product> GetAll();
    void Add(Product p);
}

public interface IOrderRepository
{
    Order? Get(Guid id);
    IEnumerable<Order> GetAll();
    void Add(Order o);
    void Update(Order o);
}

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly ConcurrentDictionary<Guid, Customer> _store = new();

    public Customer? Get(Guid id) => _store.TryGetValue(id, out var c) ? c : null;
    public IEnumerable<Customer> GetAll() => _store.Values;
    public void Add(Customer c) => _store[c.Id] = c;
}

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _store = new();
    public Product? Get(Guid id) => _store.TryGetValue(id, out var p) ? p : null;
    public IEnumerable<Product> GetAll() => _store.Values;
    public void Add(Product p) => _store[p.Id] = p;
}

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _store = new();
    public Order? Get(Guid id) => _store.TryGetValue(id, out var o) ? o : null;
    public IEnumerable<Order> GetAll() => _store.Values.OrderByDescending(o => o.CreatedUtc);
    public void Add(Order o) => _store[o.Id] = o;
    public void Update(Order o) => _store[o.Id] = o;
}

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

    // Old-style polling updates; will be turned into async streams + yield in ServicesNew.cs
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
