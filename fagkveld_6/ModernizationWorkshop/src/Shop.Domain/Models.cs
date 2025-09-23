namespace Shop.Domain;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public Customer()
    {
        Id = Guid.NewGuid();
        Name = string.Empty;
        Email = string.Empty;
    }
}

public class Product
{
    public Guid Id { get; set; }
    public string Sku { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Product()
    {
        Id = Guid.NewGuid();
        Sku = string.Empty;
        Name = string.Empty;
    }
}

public class OrderItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedUtc { get; set; }
    public List<OrderItem> Items { get; set; }
    public string Status { get; set; }

    public Order()
    {
        Id = Guid.NewGuid();
        CreatedUtc = DateTime.UtcNow;
        Items = new List<OrderItem>();
        Status = "Received";
    }
}

public class CreateOrderRequest
{
    public Guid CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public class Catalog
{
    public List<Product> Products { get; set; } = new();

    public Product? FindBySku(string sku)
    {
        return Products.FirstOrDefault(p => p.Sku.Equals(sku, StringComparison.OrdinalIgnoreCase));
    }
}
