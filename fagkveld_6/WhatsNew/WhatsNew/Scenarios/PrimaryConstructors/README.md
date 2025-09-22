# Primary Constructors

Goal: simplify DI-heavy classes by replacing verbose constructors + private fields with primary constructors.

What you have now (Before):
- Several services with many dependencies (CheckoutProcessor has 9!).
- Classic pattern with private readonly fields, null-guards, and assignments.

Your task:
1) Convert classes in `Before/Services.cs` to use primary constructors.
   - Example transformation:
     - Before:
       - `class A { private readonly IB _b; public A(IB b) { _b = b ?? throw new ArgumentNullException(nameof(b)); } void M() => _b.Do(); }`
     - After:
       - `class A(IB b) { void M() => b.Do(); }`
     - Remove the private fields and use the captured parameters directly.
     - Keep parameter names intentional and short (e.g., `repo`, `pricing`, `tax`, ...).
2) Remove redundant null checks if you prefer non-nullable types and DI guarantees.
3) Optional: turn trivial service classes into `readonly` or `sealed` as appropriate.
4) Keep behavior identical; run the demo to verify.

Tips:
- You can add validation in a body block if needed:
  - `class A(IB b) { if (b is null) throw new ArgumentNullException(nameof(b)); }`
- You can still expose properties if you want names different from parameters:
  - `class A(IB dependency) { IB Dep => dependency; }`
- Primary constructors work great with Minimal APIs and top-level statements to keep files small.

How to run:
1) Open `WhatsNew/Program.cs`.
2) Replace `RecordsDemo.Run();` with:
  `WhatsNew.Scenarios.PrimaryConstructors.PrimaryConstructorsDemo.Run();`
3) Run the app from `WhatsNew` folder.

Optional: switch between Before and After in the demo
- By default, the demo uses the Before implementations (namespace `...PrimaryConstructors.Before`).
- To show the After version, change the `using Before;` line in `PrimaryConstructorsDemo.cs` to `using After;` and re-run.

Exercises:
- Start with `CheckoutProcessor` since it has the most dependencies.
- Then update the smaller services (`OrderService`, `PricingService`, etc.).
- Discuss readability: direct parameter usage vs private fields; when to keep fields for clarity.
