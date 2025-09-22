using FluentAssertions;
using Xunit;
using System.Collections.Generic;
using WhatsNew.Scenarios.Records.New;
using Old = WhatsNew.Scenarios.Records.Before;

namespace WhatsNew.Tests;

public class RecordsTests
{
    // Desired behavior specs for Old: these FAIL now, and should pass after converting Old classes to records.

    [Fact]
    public void Old_should_have_value_equality_for_same_data()
    {
    var a = new Old.Person("Ada", new Old.Address("Stavanger", "4000"));
    var b = new Old.Person("Ada", new Old.Address("Stavanger", "4000"));
        a.Equals(b).Should().BeTrue(); // Fails until Old becomes record
    }

    [Fact]
    public void Old_HashSet_should_find_duplicate_by_value()
    {
    var a = new Old.Person("Ada", new Old.Address("Stavanger", "4000"));
    var b = new Old.Person("Ada", new Old.Address("Stavanger", "4000"));
    var set = new HashSet<Old.Person> { a };
        set.Contains(b).Should().BeTrue(); // Fails until Old becomes record
    }

    [Fact]
    public void Old_ToString_should_be_informative()
    {
    var cls = new Old.Person("Ada", new Old.Address("Stavanger", "4000"));
        cls.ToString()!.Should().Contain("Ada"); // Fails until Old becomes record (or overrides ToString)
    }

    // New behavior: these should already PASS.

    [Fact]
    public void New_value_equality_same_data_is_true()
    {
        var a = new Person("Ada", new Address("Stavanger", "4000"));
        var b = new Person("Ada", new Address("Stavanger", "4000"));
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void New_HashSet_finds_duplicate_by_value()
    {
        var a = new Person("Ada", new Address("Stavanger", "4000"));
        var b = new Person("Ada", new Address("Stavanger", "4000"));
        var set = new HashSet<Person> { a };
        set.Contains(b).Should().BeTrue();
    }

    [Fact]
    public void New_with_expression_creates_new_instance_and_preserves_original()
    {
        var a = new Person("Ada", new Address("Stavanger", "4000"));
        var b = a with { Name = "Ada Lovelace" };
        ReferenceEquals(a, b).Should().BeFalse();
        a.Name.Should().Be("Ada");
        b.Name.Should().Be("Ada Lovelace");
    }

    [Fact]
    public void New_positional_deconstruction_returns_components()
    {
        var a = new Person("Ada", new Address("Stavanger", "4000"));
        var (name, addr) = a;
        name.Should().Be("Ada");
        addr.City.Should().Be("Stavanger");
        addr.Zip.Should().Be("4000");
    }

    [Fact]
    public void New_ToString_is_informative()
    {
        var rec = new Person("Ada", new Address("Stavanger", "4000"));
        rec.ToString()!.Should().Contain("Person").And.Contain("Ada");
    }
}
