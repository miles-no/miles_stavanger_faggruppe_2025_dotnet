using Shop.Domain;

namespace Shop.Api.Seeding;

public static class DataSeeder
{
    public static Catalog SeedCatalog()
    {
        var catalog = new Catalog
        {
            Products = new List<Product>
            {
                new() { Sku = "ESP-01", Name = "Espresso", Price = 29.0m },
                new() { Sku = "LAT-01", Name = "Latte", Price = 39.0m },
                new() { Sku = "CAP-01", Name = "Cappuccino", Price = 35.0m },
            }
        };
        return catalog;
    }

    public static void SeedData(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var customers = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
        var products = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var catalog = scope.ServiceProvider.GetRequiredService<Catalog>();

        // Seed products to product repository from catalog
        foreach (var p in catalog.Products)
        {
            products.Add(p);
        }

        // Seed a default customer if none exist
        if (!customers.GetAll().Any())
        {
            var ada = new Customer { Name = "Ada Lovelace", Email = "ada@example.com" };
            customers.Add(ada);

            var demoCustomer = new CustomerWithLastOrder(ada.Name);
            NullConditionalAssignmentDemo.AssignOrder(demoCustomer);
            Console.WriteLine($"\u001b[36mDemo LastOrder status: {demoCustomer.LastOrder?.Status ?? "null"}\u001b[0m");
        }

        // Seed an initial order (idempotent)
        SeedInitialOrder(services);
    }
    
    public static void SeedInitialOrder(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var orders = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var customers = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
        var catalog = scope.ServiceProvider.GetRequiredService<Catalog>();

        var customer = customers.GetAll().FirstOrDefault();
        if (customer is null) return; 

        // Avoid duplicating if an order already exists for this customer
        if (orders.GetAll().Any(o => o.CustomerId == customer.Id)) return;

        var product = catalog.Products.FirstOrDefault();
        if (product is null) return;

        var order = new Order
        {
            CustomerId = customer.Id,
            Items = new List<OrderItem> { new() { ProductId = product.Id, Quantity = 2 } },
            Status = "Received"
        };

        orders.Add(order);
    }
}