# Modernization Workshop (C#/.NET)

This workshop starts from an "old-style" Web API using controllers and classic POCOs and guides you to modern C#/.NET features, side-by-side with solution files suffixed `New` for reference.

Timebox: ~2 hours

## What you'll modernize

- Records: convert POCOs to records with value semantics
- Performance (demo): measure materialization vs async streaming with BenchmarkDotNet
- Primary Constructors: adopt in services
- Collection Expressions: use for seeding/constants
- Escape character `\e`: console coloring banner
- Minimal APIs: move from controllers to minimal APIs
- Async Streams (`IAsyncEnumerable<T>`): stream order processing
- `Lazy<T>` (underrated): defer expensive service work
- `yield` keyword: incremental sequence generation
- C# 14 assignment target: `customer?.Order = GetOrder()` (requires preview)

## Projects

- `src/Shop.Api` — Old-style controllers (`Controllers/*.cs`) and DI in `Program.cs`
- `src/Shop.Domain` — Old models/services (`Models.cs`, `Services.cs`) and modern references (`ModelsNew.cs`, `ServicesNew.cs`)
- `tests/Shop.Tests` — A couple of tests to validate behaviors
- `benchmarks/Shop.Benchmarks` — Simple benchmarks to discuss memory vs streaming

## Prerequisites: .NET 10 / C# 14 preview

This repo is pinned to the .NET 10 SDK via `global.json` to enable C# 14 features (like null-conditional assignment targets). On macOS, install the .NET 10 SDK preview from:

- https://aka.ms/dotnet/download/dotnet/10.0

After installation, verify:

```bash
dotnet --info
```

If the SDK is not found, either install 10.x or temporarily change `<TargetFramework>` back to `net9.0` in the project files (losing C# 14 features).

## Running

1. Run API: `dotnet run --project src/Shop.Api/Shop.Api.csproj`
2. Explore routes:
   - GET `/api/customers`, POST `/api/customers`
   - GET `/api/products`, POST `/api/products`
   - GET `/api/orders`, POST `/api/orders`, POST `/api/orders/{id}/process`
3. Optional: Build Minimal API app from `ProgramNew.cs` by wiring endpoints similarly in `Program.cs` (see Tasks below).

## Tasks (with hints)

1) Records
   - Convert classes in `Models.cs` to records in your own `ModelsNew.cs` solution.
   - Use `with` expressions in tests to show immutability and value equality.

2) Primary Constructors
   - Change `PricingService` and `OrderService` to use primary constructors like `class OrderServiceNew(IOrderRepository orders, ...)`.

3) Collection Expressions
   - Replace array/list initializers with collection expressions where possible: `string[] statuses = ["Received", "Brewing", ...];`
   - Use them to seed products/customers.

4) Minimal APIs
   - Port controllers to Minimal APIs. Start from `ProgramNew.cs` as a guide; implement similar mappings in `Program.cs` (or create a new file and switch to it).

5) Async Streams + yield
   - Replace `OrderService.ProcessOrderAsync` (returns `IEnumerable<string>`) with an async stream in your `OrderServiceNew.ProcessOrderStreamAsync`.
   - Expose an endpoint that writes each step as it happens.

6) Lazy<T>
   - Make `PricingServiceNew` creation lazy-injected: `new Lazy<PricingServiceNew>(() => ...)` and use `_pricing.Value` when needed.

7) Escape `\e`
   - Add a colored banner at startup in `Program.cs` with ANSI escapes: `\e[32m` for green, `\e[0m` reset.

8) C# 14 assignment target
   - See `src/Shop.Domain/CSharp14Examples.cs` for `CustomerWithLastOrder` and `AssignLastOrderIfCustomerPresent`.
   - The key line is: `customer?.LastOrder = CreateSampleOrder();`
   - Tests in `Shop.Tests/ModernizationTests.cs` cover both the assignment and the null-skip behavior.

## Benchmarks

Run: `dotnet run -c Release --project benchmarks/Shop.Benchmarks/Shop.Benchmarks.csproj`

Discuss: materialization vs streaming. Emphasize that compiler/runtime improvements are often larger, but APIs enable different memory/throughput tradeoffs.

## Stretch goals

- Replace `Catalog.Products` with an immutable collection
- Add endpoint filters / typed results
- Add a console `Client` to show `\e` usage and call streaming endpoint
