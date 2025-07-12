using FluentAssertions;
using System.Linq.Expressions;

namespace Expose.Tests;

public class ExposeTests
{
    [Fact]
    public void Compose_NoParams()
    {
        Expression<Func<int, int>> plusOne = x => x + 1;

        var composed = ExpressionComposer.SubstituteCalls(
            () => plusOne.Call(4) + plusOne.Call(1)
        );

        var compiled = composed.Compile();
        compiled().Should().Be(7);
    }

    [Fact]
    public void Compose_OneParam()
    {
        Expression<Func<int, bool>> isNegative = x => x < 0;
        Expression<Func<int, int>> mod2 = x => x % 2;

        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => isNegative.Call(x) || mod2.Call(x) == 1
        );

        var compiled = composed.Compile();
        compiled(-1).Should().BeTrue();
        compiled(0).Should().BeFalse();
        compiled(1).Should().BeTrue();
    }

    [Fact]
    public void Compose_TwoParams()
    {
        Expression<Func<int, bool>> isNegative = x => x < 0;

        var composed = ExpressionComposer.SubstituteCalls(
            (int x, string y) => isNegative.Call(x) || y == "cat"
        );

        var compiled = composed.Compile();
        compiled(-1, "dog").Should().BeTrue();
        compiled(1, "dog").Should().BeFalse();
        compiled(1, "cat").Should().BeTrue();
    }

    [Fact]
    public void Compose_ThreeParams()
    {
        Expression<Func<int, int>> timesTwo = x => x * 2;

        var composed = ExpressionComposer.SubstituteCalls(
            (int x, int y, int z) => timesTwo.Call(x) + y + x
        );

        var compiled = composed.Compile();
        compiled(2, 3, 4).Should().Be(9);
    }

    [Fact]
    public void Compose_SimpleFunction_ExpressionEquals()
    {
        Expression<Func<int, int>> plusOne = x => x + 1;
        var composed = ExpressionComposer.SubstituteCalls((int x) => plusOne.Call(x) * 2);

        var param = Expression.Parameter(typeof(int), "x");
        Expression<Func<int, int>> expected = x => (x + 1) * 2;

        composed.ToString().Should().Be(expected.ToString());
    }

    //[Fact]
    //public void Compose_Inline_False_DoesNotInline()
    //{
    //    Expression<Func<int, int>> plusOne = x => x + 1;
    //    var visitor = new Expose.CallMethodReplacer(inline: false);
    //    var expr = (Expression<Func<int, int>>)(x => plusOne.Call(x));
    //    var visited = visitor.Visit(expr.Body);

    //    // Should be an InvokeExpression, not inlined
    //    visited.Should().BeOfType<InvocationExpression>();
    //}

    //[Fact]
    //public void Compose_Inline_True_DoesInline()
    //{
    //    Expression<Func<int, int>> plusOne = x => x + 1;
    //    var visitor = new Expose.CallMethodReplacer(inline: true);
    //    var expr = (Expression<Func<int, int>>)(x => plusOne.Call(x));
    //    var visited = visitor.Visit(expr.Body);

    //    // Should be a BinaryExpression (x + 1), not an InvokeExpression
    //    visited.Should().BeOfType<BinaryExpression>();
    //}
}
