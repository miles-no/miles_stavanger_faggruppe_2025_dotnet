// Copy/paste this into Before.cs to make the tests pass:
// - Turn classes into 'record' positional types
// - Use nested 'with' for non-destructive updates
// - Observe value equality, deconstruction, and useful ToString
namespace WhatsNew.Scenarios.Records.New;

// New way: immutable records with value-based equality and terse syntax
public record Address(string City, string Zip);

// Positional record with a mutable property redefined as init-only backing fields if needed
public record Person(string Name, Address Address)
{
    // You can still add validation or computed members
    public string City => Address.City;
}

// Inheritance with records
public record Employee(string Name, Address Address, string Department) : Person(Name, Address);
