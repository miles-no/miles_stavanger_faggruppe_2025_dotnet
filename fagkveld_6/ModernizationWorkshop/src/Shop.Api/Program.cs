using Shop.Domain;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// DI: In-memory repositories and services
builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<Catalog>(_ => SeedCatalog());
builder.Services.AddSingleton<PricingService>();
builder.Services.AddSingleton<OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map controllers (old-style baseline)
app.MapControllers();

// Seed a couple of customers and products at startup
SeedData(app.Services);

app.Run();

static Catalog SeedCatalog()
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

static void SeedData(IServiceProvider services)
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

    // Seed a default customer
    var ada = new Customer { Name = "Ada Lovelace", Email = "ada@example.com" };
    customers.Add(ada);

    var demoCustomer = new CustomerWithLastOrder(ada.Name);
    NullConditionalAssignmentDemo.AssignOrder(demoCustomer);
    Console.WriteLine($"\u001b[36mDemo (old way) LastOrder status: {demoCustomer.LastOrder?.Status ?? "null"}\u001b[0m");
}
