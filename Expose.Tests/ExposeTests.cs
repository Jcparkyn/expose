using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Expose.Tests;

public class ExposeTests
{
    private static readonly Expression<Func<int, int>> PLUS_ONE = x => x + 1;
    private static readonly Expression<Func<int, int>> TIMES_TWO = x => x * 2;
    private static readonly Expression<Func<int>> ALMOST_PI = () => 3;
    private static readonly Expression<Func<int, int, int>> MULT_2 = (x, y) => x * y;
    private static readonly Expression<Func<int, int, int, int>> MULT_3 = (x, y, z) => x * y * z;
    private static readonly Expression<Func<int, int, int, int, int>> MULT_4 = (x, y, z, j) => x * y * z * j;
    private static Expression<Func<int, int>> PlusOneProperty { get; } = x => x + 1;

    [Fact]
    public void SubstituteCalls_NoParamsOut()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            () => PLUS_ONE.Call(4) + TIMES_TWO.Call(1)
        );

        var compiled = composed.Compile();
        compiled().Should().Be(7);
    }

    [Fact]
    public void SubstituteCalls_OneParamOut()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => TIMES_TWO.Call(x) + 1
        );

        composed.ToString().Should().Be("x => ((x * 2) + 1)");
        composed.Compile()(7).Should().Be(15);
    }

    [Fact]
    public void SubstituteCalls_TwoParamsOut()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x, int y) => TIMES_TWO.Call(x) + y
        );

        composed.ToString().Should().Be("(x, y) => ((x * 2) + y)");
        composed.Compile()(1, 2).Should().Be(4);
    }

    [Fact]
    public void SubstituteCalls_ThreeParamsOut()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x, int y, int z) => TIMES_TWO.Call(x) + y + z
        );

        composed.ToString().Should().Be("(x, y, z) => (((x * 2) + y) + z)");
        composed.Compile()(2, 3, 4).Should().Be(11);
    }

    [Fact]
    public void SubstituteCalls_NoParamsIn()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => ALMOST_PI.Call() + x
        );

        composed.ToString().Should().Be("x => (3 + x)");
        composed.Compile()(7).Should().Be(10);
    }

    [Fact]
    public void SubstituteCalls_OneParamIn()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => TIMES_TWO.Call(x) + 1
        );

        composed.ToString().Should().Be("x => ((x * 2) + 1)");
        composed.Compile()(7).Should().Be(15);
    }

    [Fact]
    public void SubstituteCalls_TwoParamsIn()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => MULT_2.Call(x, 3) + 1
        );

        composed.ToString().Should().Be("x => ((x * 3) + 1)");
        composed.Compile()(7).Should().Be(22);
    }

    [Fact]
    public void SubstituteCalls_ThreeParamsIn()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => MULT_3.Call(x, 3, x + 1) + 1
        );

        composed.ToString().Should().Be("x => (((x * 3) * (x + 1)) + 1)");
        composed.Compile()(7).Should().Be(169);
    }

    [Fact]
    public void SubstituteCalls_FourParamsIn()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => MULT_4.Call(x, 3, x + 1, x + 2) + 1
        );

        composed.ToString().Should().Be("x => ((((x * 3) * (x + 1)) * (x + 2)) + 1)");
        composed.Compile()(7).Should().Be(1513);
    }

    [Fact]
    public void ShouldInlineLocalExpressionVar()
    {
        Expression<Func<int, int>> plusOne = x => x + 1;
        var composed = ExpressionComposer.SubstituteCalls(
            () => plusOne.Call(4)
        );

        composed.ToString().Should().Be("() => (4 + 1)");
        var compiled = composed.Compile();
        compiled().Should().Be(5);
    }

    [Fact]
    public void ShouldInlineProperty()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            () => PlusOneProperty.Call(4)
        );

        composed.ToString().Should().Be("() => (4 + 1)");
        var compiled = composed.Compile();
        compiled().Should().Be(5);
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

    //private static void BeSameExpressionAs<T>(ObjectAssertions should, Expression<T> expected)
    //{
    //    should.NotBeSameAs;
    //}
}
