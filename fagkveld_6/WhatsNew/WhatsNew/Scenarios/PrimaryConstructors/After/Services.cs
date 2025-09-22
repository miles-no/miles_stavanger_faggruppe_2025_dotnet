namespace WhatsNew.Scenarios.PrimaryConstructors.After;

// Domain models (could also be records if desired). Kept as simple classes here.
public sealed class Product(string id, string name, decimal price)
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public decimal BasePrice { get; } = price;
}

public sealed class CartItem(string productId, int quantity)
{
    public string ProductId { get; } = productId;
    public int Quantity { get; } = quantity;
}

public sealed class Order(string id, IReadOnlyList<CartItem> items, decimal total)
{
    public string Id { get; } = id;
    public IReadOnlyList<CartItem> Items { get; } = items;
    public decimal Total { get; } = total;
}


// Implementations (After: primary constructors, no private fields)
public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly Dictionary<string, Product> _db = new()
    {
        ["p1"] = new Product("p1", "Keyboard", 499),
        ["p2"] = new Product("p2", "Mouse", 299),
        ["p3"] = new Product("p3", "Monitor", 2499)
    };

    public Product GetById(string id)
        => _db.TryGetValue(id, out var p) ? p : throw new KeyNotFoundException(id);
}

public sealed class PricingService(IProductRepository products, ILogger log) : IPricingService
{
    public decimal CalculateSubtotal(IEnumerable<CartItem> items)
    {
        decimal sum = 0;
        foreach (var i in items)
        {
            var p = products.GetById(i.ProductId);
            sum += p.BasePrice * i.Quantity;
        }
        log.Info($"Subtotal calculated: {sum}");
        return sum;
    }
}

public sealed class TaxService(ILogger log) : ITaxService
{
    public decimal CalculateTax(decimal subTotal, string countryCode)
    {
        var rate = countryCode == "NO" ? 0.25m : 0.2m;
        var tax = Math.Round(subTotal * rate, 2);
        log.Info($"Tax for {countryCode} at {rate:P0}: {tax}");
        return tax;
    }
}

public sealed class DiscountService(ILogger log) : IDiscountService
{
    public decimal ApplyDiscounts(decimal amount, string? couponCode)
    {
        var discount = couponCode == "SAVE10" ? 0.10m : 0m;
        var res = Math.Round(amount * (1 - discount), 2);
        if (discount > 0) log.Info($"Applied {discount:P0} discount -> {res}");
        return res;
    }
}

public sealed class ShippingService(ILogger log) : IShippingService
{
    public decimal GetShipping(decimal amount, string countryCode)
    {
        var ship = amount > 2000 ? 0 : (countryCode == "NO" ? 99 : 149);
        log.Info($"Shipping: {ship}");
        return ship;
    }
}

public sealed class FakePaymentGateway(ILogger log) : IPaymentGateway
{
    public bool Charge(string customerId, decimal amount)
    {
        log.Info($"Charging {customerId} NOK {amount}");
        return true;
    }
}

public sealed class InMemoryOrderRepository(ILogger log) : IOrderRepository
{
    private readonly List<Order> _orders = new();

    public string Save(Order order)
    {
        _orders.Add(order);
        log.Info($"Saved order {order.Id} with total {order.Total}");
        return order.Id;
    }
}

public sealed class ConsoleEmailService(ILogger log) : IEmailService
{
    public void SendReceipt(string email, Order order)
        => log.Info($"Emailed receipt to {email} for order {order.Id}");
}

public sealed class ConsoleLogger : ILogger
{
    public void Info(string message) => Console.WriteLine($"[INFO]  {message}");
    public void Error(string message) => Console.WriteLine($"[ERROR] {message}");
}

// Orchestrator (After): primary constructor with many deps, directly used
public sealed class CheckoutProcessor(
    IPricingService pricing,
    ITaxService tax,
    IDiscountService discounts,
    IShippingService shipping,
    IPaymentGateway payments,
    IOrderRepository orders,
    IEmailService email,
    ILogger log)
{
    public string Checkout(
        string customerId,
        string emailAddress,
        string countryCode,
        IEnumerable<CartItem> items,
        string? couponCode)
    {
        var sub = pricing.CalculateSubtotal(items);
        var t = tax.CalculateTax(sub, countryCode);
        var discounted = discounts.ApplyDiscounts(sub + t, couponCode);
        var ship = shipping.GetShipping(discounted, countryCode);
        var total = discounted + ship;

        if (!payments.Charge(customerId, total))
        {
            log.Error($"Payment failed for {customerId}");
            throw new InvalidOperationException("Payment failed");
        }

        var order = new Order(Guid.NewGuid().ToString("N"), items.ToList(), total);
        var id = orders.Save(order);
        email.SendReceipt(emailAddress, order);
        log.Info($"Order complete id={id}, total={total}");
        return id;
    }
}
