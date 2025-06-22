# Expose

Expose is a small C# library for combining and composing expression trees, with full type safety and zero dependencies.

This can be used with LINQ, EF Core, and anything else that works with expression trees.

## Installation

I haven't published this on NuGet yet. For now, just copy-paste the file Expose.cs into your project.

## Usage

**Composing expressions:**

```cs
using Expose;

Expression<Func<int, bool>> isNegative = x => x < 0;
Expression<Func<int, int>> mod2 = x => x % 2;

Expression<Func<int, bool>> composed = ExpressionComposer.SubstituteCalls(
    // This replaces the .Call() usages with the actual expressions.
    (int x) => isNegative.Call(x) || mod2.Call(x) == 1
);
```

**Optional: Shorthand usage with LINQ and/or EF Core**

```cs
using Expose;

Expression<Func<int, bool>> isNegative = x => x < 0;
Expression<Func<int, int>> mod2 = x => x % 2;

using var context = new MyDbContext(options);
var result = context.MyEntities
    .Where(e => isNegative.Call(e.Age) || mod2.Call(e.Age) == 1)
    // This replaces the .Call() usages with the actual expressions.
    // You can chain multiple LINQ methods using .Call() before you call .SubstituteCalls().
    .SubstituteCalls()
    .Single();
```
