# Records – old vs new

Goal: Show how records reduce boilerplate, provide value-based equality, and enable immutable, non-destructive updates with `with`.

Talking points
- Records are reference types with value equality by default.
- Auto-generated `Equals`, `GetHashCode`, `Deconstruct`, and `ToString`.
- Great for DTOs, messages, results, and state snapshots.
- Non-destructive updates via `with` expressions (including nested).
- Prefer classes when you need identity semantics or mutable entities (e.g., EF Core tracked entities).

Exercise (10–15 min)
1. Run the app and read the output for the records demo.
2. Convert `Old.Person` and `Old.Address` classes to `record` types; remove custom `Equals/GetHashCode`.
3. Add a new property (e.g., `int Age`) and observe how equality and `ToString` behave automatically.
4. Use a `with` expression to change only the city on a nested address: `var p2 = p1 with { Address = p1.Address with { City = "Bergen" } };`.
5. Discuss tradeoffs and when to choose classes vs records.

Files
- `Old.cs` – classic classes with manual equality and mutability.
- `New.cs` – records version with positional syntax and inheritance.
- `RecordsDemo.cs` – prints differences and suggested exercises.
