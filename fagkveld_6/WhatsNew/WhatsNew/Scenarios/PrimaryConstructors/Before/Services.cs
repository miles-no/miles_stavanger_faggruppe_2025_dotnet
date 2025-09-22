namespace WhatsNew.Scenarios.PrimaryConstructors.Before;

// Domain models (Before: classic constructors, no primary constructors yet)
public sealed class Product
{
    public string Id { get; }
    public string Name { get; }
    public decimal BasePrice { get; }

    public Product(string id, string name, decimal price)
    {
        Id = id;
        Name = name;
        BasePrice = price;
    }
}

public sealed class CartItem
{
    public string ProductId { get; }
    public int Quantity { get; }

    public CartItem(string productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}

public sealed class Order
{
    public string Id { get; }
    public IReadOnlyList<CartItem> Items { get; }
    public decimal Total { get; }

    public Order(string id, IReadOnlyList<CartItem> items, decimal total)
    {
        Id = id;
        Items = items;
        Total = total;
    }
}

// Abstractions
public interface IProductRepository { Product GetById(string id); }
public interface IPricingService { decimal CalculateSubtotal(IEnumerable<CartItem> items); }
public interface ITaxService { decimal CalculateTax(decimal subTotal, string countryCode); }
public interface IDiscountService { decimal ApplyDiscounts(decimal amount, string? couponCode); }
public interface IShippingService { decimal GetShipping(decimal amount, string countryCode); }
public interface IPaymentGateway { bool Charge(string customerId, decimal amount); }
public interface IOrderRepository { string Save(Order order); }
public interface IEmailService { void SendReceipt(string email, Order order); }
public interface ILogger { void Info(string message); void Error(string message); }

// Implementations (simple/in-memory)
public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly Dictionary<string, Product> _db = new()
    {
        ["p1"] = new Product("p1", "Keyboard", 499),
        ["p2"] = new Product("p2", "Mouse", 299),
        ["p3"] = new Product("p3", "Monitor", 2499)
    };

    public Product GetById(string id)
    {
        if (!_db.TryGetValue(id, out var p)) throw new KeyNotFoundException(id);
        return p;
    }
}

public sealed class PricingService : IPricingService
{
    private readonly IProductRepository _products;
    private readonly ILogger _log;

    public PricingService(IProductRepository products, ILogger log)
    {
        _products = products ?? throw new ArgumentNullException(nameof(products));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public decimal CalculateSubtotal(IEnumerable<CartItem> items)
    {
        decimal sum = 0;
        foreach (var i in items)
        {
            var p = _products.GetById(i.ProductId);
            sum += p.BasePrice * i.Quantity;
        }
        _log.Info($"Subtotal calculated: {sum}");
        return sum;
    }
}

public sealed class TaxService : ITaxService
{
    private readonly ILogger _log;

    public TaxService(ILogger log)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public decimal CalculateTax(decimal subTotal, string countryCode)
    {
        var rate = countryCode == "NO" ? 0.25m : 0.2m;
        var tax = Math.Round(subTotal * rate, 2);
        _log.Info($"Tax for {countryCode} at {rate:P0}: {tax}");
        return tax;
    }
}

public sealed class DiscountService : IDiscountService
{
    private readonly ILogger _log;

    public DiscountService(ILogger log)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public decimal ApplyDiscounts(decimal amount, string? couponCode)
    {
        var discount = couponCode == "SAVE10" ? 0.10m : 0m;
        var res = Math.Round(amount * (1 - discount), 2);
        if (discount > 0) _log.Info($"Applied {discount:P0} discount -> {res}");
        return res;
    }
}

public sealed class ShippingService : IShippingService
{
    private readonly ILogger _log;

    public ShippingService(ILogger log)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public decimal GetShipping(decimal amount, string countryCode)
    {
        var ship = amount > 2000 ? 0 : (countryCode == "NO" ? 99 : 149);
        _log.Info($"Shipping: {ship}");
        return ship;
    }
}

public sealed class FakePaymentGateway : IPaymentGateway
{
    private readonly ILogger _log;

    public FakePaymentGateway(ILogger log)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public bool Charge(string customerId, decimal amount)
    {
        _log.Info($"Charging {customerId} NOK {amount}");
        return true;
    }
}

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ILogger _log;
    private readonly List<Order> _orders = new();

    public InMemoryOrderRepository(ILogger log)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public string Save(Order order)
    {
        _orders.Add(order);
        _log.Info($"Saved order {order.Id} with total {order.Total}");
        return order.Id;
    }
}

public sealed class ConsoleEmailService : IEmailService
{
    private readonly ILogger _log;

    public ConsoleEmailService(ILogger log)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public void SendReceipt(string email, Order order)
    {
        _log.Info($"Emailed receipt to {email} for order {order.Id}");
    }
}

public sealed class ConsoleLogger : ILogger
{
    public void Info(string message) => Console.WriteLine($"[INFO]  {message}");
    public void Error(string message) => Console.WriteLine($"[ERROR] {message}");
}

// Orchestrator with MANY dependencies – perfect for primary constructors
public sealed class CheckoutProcessor
{
    private readonly IPricingService _pricing;
    private readonly ITaxService _tax;
    private readonly IDiscountService _discounts;
    private readonly IShippingService _shipping;
    private readonly IPaymentGateway _payments;
    private readonly IOrderRepository _orders;
    private readonly IEmailService _email;
    private readonly ILogger _log;

    public CheckoutProcessor(
        IPricingService pricing,
        ITaxService tax,
        IDiscountService discounts,
        IShippingService shipping,
        IPaymentGateway payments,
        IOrderRepository orders,
        IEmailService email,
        ILogger log)
    {
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _tax = tax ?? throw new ArgumentNullException(nameof(tax));
        _discounts = discounts ?? throw new ArgumentNullException(nameof(discounts));
        _shipping = shipping ?? throw new ArgumentNullException(nameof(shipping));
        _payments = payments ?? throw new ArgumentNullException(nameof(payments));
        _orders = orders ?? throw new ArgumentNullException(nameof(orders));
        _email = email ?? throw new ArgumentNullException(nameof(email));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public string Checkout(
        string customerId,
        string email,
        string countryCode,
        IEnumerable<CartItem> items,
        string? couponCode)
    {
        var sub = _pricing.CalculateSubtotal(items);
        var tax = _tax.CalculateTax(sub, countryCode);
        var discounted = _discounts.ApplyDiscounts(sub + tax, couponCode);
        var ship = _shipping.GetShipping(discounted, countryCode);
        var total = discounted + ship;

        var ok = _payments.Charge(customerId, total);
        if (!ok)
        {
            _log.Error($"Payment failed for {customerId}");
            throw new InvalidOperationException("Payment failed");
        }

        var order = new Order(Guid.NewGuid().ToString("N"), items.ToList(), total);
        var id = _orders.Save(order);
        _email.SendReceipt(email, order);
        _log.Info($"Order complete id={id}, total={total}");
        return id;
    }
}
