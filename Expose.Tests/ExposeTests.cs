using Expose;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xunit;

namespace Expose.Tests;

public class ExposeTests
{
    [Fact]
    public void Then()
    {
        Expression<Func<int, string>> first = x => x.ToString();
        var composed = first.Then(x => x + "!");
        var input = 4;
        var output = composed.Compile().Invoke(input);
        output.Should().Be("4!");
    }

    [Fact]
    public void Compose()
    {
        Expression<Func<int, bool>> isNegative = x => x < 0;
        Expression<Func<int, int>> mod2 = x => x % 2;

        var wrapped = Expose.Compose(
            (int x) => isNegative.Call(x) || mod2.Call(x) == 1
        );

        var compiled = wrapped.Compile();
        compiled.Invoke(-1).Should().BeTrue();
        compiled.Invoke(0).Should().BeFalse();
        compiled.Invoke(1).Should().BeTrue();
    }
}
