namespace Shop.Domain;

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
    public static void AssignOrder(CustomerWithLastOrder? customer)
    {
        if (customer is not null)
        {
            customer.LastOrder = CreateSampleOrder();
        }
    }

    public static Order CreateSampleOrder()
        => new() { Status = "Demo" };
}