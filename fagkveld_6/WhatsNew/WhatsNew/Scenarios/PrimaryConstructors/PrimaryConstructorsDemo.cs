namespace WhatsNew.Scenarios.PrimaryConstructors;

using Before;

public static class PrimaryConstructorsDemo
{
    public static void Run()
    {
        WriteHeader("Primary constructors: convert 'Before' to primary constructors to reduce boilerplate.\n");

        // Compose services manually (simulating DI container build)
        var log = new Before.ConsoleLogger();
        var products = new Before.InMemoryProductRepository();
        var pricing = new Before.PricingService(products, log);
        var tax = new Before.TaxService(log);
        var discounts = new Before.DiscountService(log);
        var shipping = new Before.ShippingService(log);
        var payments = new Before.FakePaymentGateway(log);
        var orders = new Before.InMemoryOrderRepository(log);
        var email = new Before.ConsoleEmailService(log);
        var checkout = new Before.CheckoutProcessor(pricing, tax, discounts, shipping, payments, orders, email, log);

        // Prepare a tiny cart
        var items = new[]
        {
            new Before.CartItem("p1", 1),
            new Before.CartItem("p2", 2)
        };

        var orderId = checkout.Checkout(
            customerId: "cust-123",
            email: "ada@example.com",
            countryCode: "NO",
            items: items,
            couponCode: "SAVE10");

        Console.WriteLine($"Created order id: {orderId}");

        WriteExercises();
    }

    private static void WriteHeader(string text)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("-- " + text);
        Console.ResetColor();
    }

    private static void WriteExercises()
    {
        Console.WriteLine("Exercises (convert Before -> primary constructors):\n" +
                          "1) Convert CheckoutProcessor to `class CheckoutProcessor(IPricingService pricing, ...)` and remove fields.\n" +
                          "2) Convert PricingService, TaxService, etc. to primary constructors as well.\n" +
                          "3) Optional: remove null-guards if you rely on DI + non-nullable refs.\n" +
                          "4) Discuss clarity: when are private fields still preferable?\n");
    }
}
