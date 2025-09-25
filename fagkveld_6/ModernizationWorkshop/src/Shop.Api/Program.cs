using Shop.Domain;
using Shop.Api.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// DI: In-memory repositories and services
builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<Catalog>(_ => DataSeeder.SeedCatalog());
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
DataSeeder.SeedData(app.Services);

app.Run();

// Seeding methods moved to DataSeeder in Seeding/DataSeeder.cs
