namespace WhatsNew.Scenarios.Records.Before;

// Starting point: classic classes (reference equality, mutable, unhelpful ToString)
public class Address
{
    public string City { get; set; }
    public string Zip { get; set; }

    public Address(string city, string zip)
    {
        City = city;
        Zip = zip;
    }
}

public class Person
{
    public string Name { get; set; }
    public Address Address { get; set; }

    public Person(string name, Address address)
    {
        Name = name;
        Address = address;
    }
}