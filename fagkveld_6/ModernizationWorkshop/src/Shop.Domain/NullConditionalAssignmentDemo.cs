namespace Shop.Domain;

// Demonstrates C# 14 null-conditional assignment target: customer?.LastOrder = GetOrder();
public class CustomerWithLastOrder
{
    public CustomerWithLastOrder(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public Order? LastOrder { get; set; }
}

public static class NullConditionalAssignmentDemo
{
    // C# 14: null-conditional assignment target
    // Before C# 14, you needed to null-check before assigning:
    public static void AssignOrderOldWay(CustomerWithLastOrder? customer)
    {
        if (customer is not null)
        {
            customer.LastOrder = CreateSampleOrder();
        }
    }

    // C# 14: You can simplify using the ?. operator on the left side
    public static void AssignOrderNewWay(CustomerWithLastOrder? customer)
    {
        // The right side is evaluated only when the left side isn't null.
        // If customer is null, CreateSampleOrder() is not called.
        customer?.LastOrder = CreateSampleOrder();
    }

    public static Order CreateSampleOrder()
        => new() { Status = "Demo" };
}