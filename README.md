# Expose&nbsp; [![Nuget](https://img.shields.io/nuget/v/Expose)](https://www.nuget.org/packages/Expose) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Expose is a small C# library for composing expression trees, with full type safety and zero dependencies.
It allows you to create complex expressions by calling other expressions, making it easier to build dynamic queries in a simple and intuitive way.

This can be used with LINQ, EF Core, and anything else that works with expression trees.

## Installing

You can install Expose via the .NET CLI with the following command:

```sh
dotnet add package Expose
```

If you're using Visual Studio, you can also install via the built-in NuGet package manager.

## Usage

#### Composing expressions:

```cs
using Expose;

Expression<Func<int, bool>> isNegative = x => x < 0;
Expression<Func<int, int>> mod2 = x => x % 2;

Expression<Func<int, bool>> composed = ExpressionComposer.SubstituteCalls(
    // This replaces the .CallInline() usages with the actual expressions.
    (int x) => isNegative.CallInline(x) || mod2.CallInline(x) == 1
);
```

#### Shorthand usage with LINQ and/or EF Core

To avoid having to repeat `ExpressionComposer.SubstituteCalls` and specify types explicitly, you can use `SubstituteCalls()` on an `IQueryable` to automatically substitute all previous usages of `CallInline` or `CallInvoke`.

```cs
using Expose;

Expression<Func<int, bool>> isNegative = x => x < 0;
Expression<Func<int, int>> mod2 = x => x % 2;

using var context = new MyDbContext(options);
var result = await context.MyEntities
    .Where(e => isNegative.CallInline(e.Age) || mod2.CallInline(e.Age) == 1)
    // This replaces the .CallInline() usages with the actual expressions.
    // You can chain multiple LINQ methods using .CallInline() before you call .SubstituteCalls().
    .SubstituteCalls()
    .ToListAsync();
```

#### Composing without inlining:

If you don't want to inline the nested methods, you can use `CallInvoke()` instead of `CallInline()`.
This will replace them with an `InvokationExpression` instead of inlining the method body.
This may may improve compatibility with complex expressions that can't/shouldn't be inlined, but could reduce compatibility with other libraries that don't know how to interpret an `InvokationExpression` node.

## Similar Libraries

After writing this library, I found two other projects using similar approaches:
1. [LINQKit](https://github.com/scottksmith95/LINQKit): Their `Invoke()` method is roughly equivalent to my `CallInline()` method, but rather than substituting calls at the end, they use `AsExpandable()` up-front which replaces calls when the IQueryable gets evaluated.
1. A [blog post by balefrost](https://balefrost.github.io/expression_splicing.html) which uses the names "Inline" and "Splice". This doesn't include any shorthands for working with `IQueryable`.

## Contributing

Any contributions are welcome, but ideally start by creating an [issue](https://github.com/Jcparkyn/expose/issues).
