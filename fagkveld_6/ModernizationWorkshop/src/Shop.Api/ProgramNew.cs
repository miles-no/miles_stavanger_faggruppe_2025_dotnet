using Microsoft.AspNetCore.Http.HttpResults;
using Shop.Domain;

namespace Shop.Api;

// Minimal API version of the app (solution reference). Attendees will port Program.cs to this style.
public static class ProgramNew
{
    public static WebApplication BuildApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
        builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
        builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        builder.Services.AddSingleton<Catalog>(_ => SeedCatalog());
        builder.Services.AddSingleton<PricingServiceNew>();
        builder.Services.AddSingleton(sp => new Lazy<PricingServiceNew>(() => sp.GetRequiredService<PricingServiceNew>()));
        builder.Services.AddSingleton<OrderServiceNew>();

        var app = builder.Build();
        if (app.Environment.IsDevelopment()) app.MapOpenApi();

        // Collection expression example for constants
        string[] statuses = ["Received", "Brewing", "Packing", "Ready"]; // C# 12

        app.MapGet("/api/min/customers", (ICustomerRepository repo) => Results.Ok(repo.GetAll()));

        app.MapPost("/api/min/orders", (CreateOrderRequestNew req, OrderServiceNew svc) =>
        {
            var created = svc.Create(req);
            return Results.Created($"/api/min/orders/{created.Id}", created);
        });

        app.MapGet("/api/min/orders/{id:guid}/stream", async Task<Results<NotFound, IResult>> (Guid id, OrderServiceNew svc, HttpContext ctx) =>
        {
            try
            {
                ctx.Response.Headers.CacheControl = "no-store";
                await foreach (var step in svc.ProcessOrderStreamAsync(id, ctx.RequestAborted))
                {
                    await ctx.Response.WriteAsync(step + "\n");
                    await ctx.Response.Body.FlushAsync();
                }
                return TypedResults.Ok(Results.Text("done"));
            }
            catch (InvalidOperationException)
            {
                return TypedResults.NotFound();
            }
        });

        // Example of C# 14 null-conditional assignment target (to be enabled in preview SDK/environment)
        // TODO(Workshop): When using preview C# 14, demonstrate:
        // customer?.Order = GetOrder();
        // We'll include a small CustomerWithLastOrder type in tests to exercise this.

        return app;
    }

    private static Catalog SeedCatalog()
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
}
