namespace WhatsNew.Scenarios.Records;

public static class RecordsDemo
{
    public static void Run()
    {
        WriteHeader("Records: convert 'Before' to records to turn these checks GREEN\n");

        // Arrange
        var old1 = new Before.Person("Ada", new Before.Address("Stavanger", "4000"));
        var old2 = new Before.Person("Ada", new Before.Address("Stavanger", "4000"));

        // 1) Equality check (should become TRUE after conversion to records)
        var eq = old1.Equals(old2);
        CheckExpectedTrue(
            "Value equality for same data",
            expectedText: "True (same data -> value-equal)",
            actualText: $"Equals={eq}; hash(old1)={old1.GetHashCode()}, hash(old2)={old2.GetHashCode()}",
            actual: eq
        );

        // 2) Hash-based collection behavior (should become TRUE after conversion to records)
        var set = new HashSet<Before.Person> { old1 };
        var addedDuplicate = set.Add(old2);
        CheckExpectedTrue(
            "HashSet finds duplicate by value",
            expectedText: "Add duplicate returns False; Count=1",
            actualText: $"Add returned {addedDuplicate}; Count={set.Count}",
            actual: addedDuplicate == false && set.Count == 1
        );

        // 3) Dictionary key behavior (should become TRUE after conversion to records)
        var dict = new Dictionary<Before.Person, string> { [old1] = "original" };
        var dictLookupWorks = dict.TryGetValue(old2, out var dictVal);
        CheckExpectedTrue(
            "Dictionary key lookup by value",
            expectedText: "True; value='original'",
            actualText: $"{dictLookupWorks}; value='{dictVal}'",
            actual: dictLookupWorks && dictVal == "original"
        );

        // 4) ToString usefulness (should include values after conversion to records)
        var actualToString = old1.ToString() ?? string.Empty;
        Console.WriteLine("  ToString:  " + actualToString);
        var toStringOk = actualToString.Contains(old1.Name)
                         && actualToString.Contains(old1.Address.City)
                         && actualToString.Contains(old1.Address.Zip);
        CheckExpectedTrue(
            "ToString includes values",
            expectedText: $"includes Name='{old1.Name}', City='{old1.Address.City}', Zip='{old1.Address.Zip}'",
            actualText: actualToString,
            actual: toStringOk
        );

        // 5) Mutability contrast: we EXPECT init-only after converting to positional records
        // Instead of trying to set the value (which reflection might allow), we inspect metadata for init-only
        var hasInitOnlySetter = HasInitOnlySetter(old1.Address, "City");
        CheckExpectedTrue(
            "Address.City is init-only",
            expectedText: "Setter marked init-only",
            actualText: $"init-only={hasInitOnlySetter}",
            actual: hasInitOnlySetter
        );


        // 6) Deconstruction: records synthesize Deconstruct(out ...) methods for positional components
        var deconstructedPerson = TryDeconstruct(old1, out var comps1) && comps1.Length == 2;
        var nameOk = deconstructedPerson && comps1[0] is string n && n == "Ada";
        var addrOk = false;
        Before.Address addr = null!;
        if (deconstructedPerson && comps1[1] is Before.Address a)
        {
            addrOk = true;
            addr = a;
        }
        var deconstructedAddress = false;
        object[] comps2 = Array.Empty<object>();
        if (addrOk && TryDeconstruct(addr, out comps2) && comps2.Length == 2)
        {
            deconstructedAddress = true;
        }
        var cityOk = deconstructedAddress && comps2[0] is string c && c == "Stavanger";
        var zipOk = deconstructedAddress && comps2[1] is string z && z == "4000";
        CheckExpectedTrue(
            "Positional deconstruction methods exist",
            expectedText: "Person.Deconstruct and Address.Deconstruct available with expected values",
            actualText: $"person={deconstructedPerson}, nameOk={nameOk}, address={addrOk}, cityOk={cityOk}, zipOk={zipOk}",
            actual: deconstructedPerson && nameOk && addrOk && deconstructedAddress && cityOk && zipOk
        );

        // 8) IEquatable<T>: records implement typed equality automatically
        var implementsEquatable = typeof(Before.Person).GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEquatable<>) && i.GenericTypeArguments[0] == typeof(Before.Person));
        CheckExpectedTrue(
            "Implements IEquatable<Person>",
            expectedText: "True",
            actualText: implementsEquatable.ToString(),
            actual: implementsEquatable
        );

        // 9) PrintMembers: records provide a PrintMembers(StringBuilder) used by ToString
        var hasPrintMembers = HasPrintMembers(typeof(Before.Person));
        CheckExpectedTrue(
            "Has PrintMembers for ToString",
            expectedText: "True",
            actualText: hasPrintMembers.ToString(),
            actual: hasPrintMembers
        );

        Console.WriteLine();
        WriteTips();
        WriteExercises();
    }

    private static void Check(string label, bool expected, bool actual)
        => Check(label, expected ? "PASS" : "FAIL", actual ? "PASS" : "FAIL");

    private static void Check<T>(string label, T expected, T actual)
    {
        var ok = Equals(expected, actual);
        SetColor(ok ? ConsoleColor.Green : ConsoleColor.Red);
        Console.WriteLine($"[{(ok ? "PASS" : "FAIL")}] {label}");
        ResetColor();
        Console.WriteLine($"  expected: {expected}\n  actual:   {actual}\n");
    }

    private static void WriteHeader(string text)
    {
        SetColor(ConsoleColor.Cyan);
        Console.WriteLine("-- " + text);
        ResetColor();
    }

    private static void WriteTips()
    {
        Console.WriteLine("Why records?\n- Value equality by default (DTOs, messages, state)\n- Auto ToString/Deconstruct/Equals/GetHashCode (less boilerplate)\n- Prefer classes when identity/mutability are required\n");
    }

    private static void WriteExercises()
    {
        Console.WriteLine("Exercises (convert Before -> record and re-run):\n1) Change Before.Address and Before.Person to records (positional recommended).\n2) Add Age to Person; observe ToString/equality update automatically.\n3) Discuss when classes are preferable (EF Core tracked entities, identity).\n");
    }

    private static void SetColor(ConsoleColor color) => Console.ForegroundColor = color;
    private static void ResetColor() => Console.ResetColor();

    private static void CheckExpectedTrue(string label, string expectedText, string actualText, bool actual)
    {
        var ok = actual;
        SetColor(ok ? ConsoleColor.Green : ConsoleColor.Red);
        Console.WriteLine($"[{(ok ? "PASS" : "FAIL")}] {label}");
        ResetColor();
        Console.WriteLine($"  expected: {expectedText}\n  actual:   {actualText}\n");
    }

    private static bool HasInitOnlySetter(object obj, string propertyName)
    {
        var type = obj.GetType();
        var prop = type.GetProperty(propertyName);
        if (prop == null) return false;
        var setter = prop.SetMethod;
        if (setter == null) return false; // no setter at all

        // Detect C# init-only via required custom modifier IsExternalInit on the setter's return parameter
        var mods = setter.ReturnParameter.GetRequiredCustomModifiers();
        foreach (var m in mods)
        {
            if (m.FullName == "System.Runtime.CompilerServices.IsExternalInit")
                return true;
        }
        return false;
    }

    private static bool TryDeconstruct(object obj, out object[] components)
    {
        components = Array.Empty<object>();
        var type = obj.GetType();
        var method = type.GetMethod("Deconstruct", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (method == null) return false;
        var parameters = method.GetParameters();
        if (parameters.Length == 0) return false;
        var args = new object?[parameters.Length];
        try
        {
            method.Invoke(obj, args!);
            components = args!;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool HasPrintMembers(Type type)
    {
        var method = type.GetMethod(
            "PrintMembers",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(System.Text.StringBuilder) },
            modifiers: null);
        return method != null && method.ReturnType == typeof(bool);
    }
}
