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
            () => PLUS_ONE.CallInline(4) + TIMES_TWO.CallInline(1)
        );

        var compiled = composed.Compile();
        compiled().Should().Be(7);
    }

    [Fact]
    public void SubstituteCalls_OneParamOut()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => TIMES_TWO.CallInline(x) + 1
        );

        composed.ToString().Should().Be("x => ((x * 2) + 1)");
        composed.Compile()(7).Should().Be(15);
    }

    [Fact]
    public void SubstituteCalls_TwoParamsOut()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x, int y) => TIMES_TWO.CallInline(x) + y
        );

        composed.ToString().Should().Be("(x, y) => ((x * 2) + y)");
        composed.Compile()(1, 2).Should().Be(4);
    }

    [Fact]
    public void SubstituteCalls_ThreeParamsOut()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x, int y, int z) => TIMES_TWO.CallInline(x) + y + z
        );

        composed.ToString().Should().Be("(x, y, z) => (((x * 2) + y) + z)");
        composed.Compile()(2, 3, 4).Should().Be(11);
    }

    [Fact]
    public void SubstituteCalls_NoParamsIn()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => ALMOST_PI.CallInline() + x
        );

        composed.ToString().Should().Be("x => (3 + x)");
        composed.Compile()(7).Should().Be(10);
    }

    [Fact]
    public void SubstituteCalls_OneParamIn()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => TIMES_TWO.CallInline(x) + 1
        );

        composed.ToString().Should().Be("x => ((x * 2) + 1)");
        composed.Compile()(7).Should().Be(15);
    }

    [Fact]
    public void SubstituteCalls_TwoParamsIn()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => MULT_2.CallInline(x, 3) + 1
        );

        composed.ToString().Should().Be("x => ((x * 3) + 1)");
        composed.Compile()(7).Should().Be(22);
    }

    [Fact]
    public void SubstituteCalls_ThreeParamsIn()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => MULT_3.CallInline(x, 3, x + 1) + 1
        );

        composed.ToString().Should().Be("x => (((x * 3) * (x + 1)) + 1)");
        composed.Compile()(7).Should().Be(169);
    }

    [Fact]
    public void SubstituteCalls_FourParamsIn()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => MULT_4.CallInline(x, 3, x + 1, x + 2) + 1
        );

        composed.ToString().Should().Be("x => ((((x * 3) * (x + 1)) * (x + 2)) + 1)");
        composed.Compile()(7).Should().Be(1513);
    }

    [Fact]
    public void ShouldInlineLocalExpressionVar()
    {
        Expression<Func<int, int>> plusOne = x => x + 1;
        var composed = ExpressionComposer.SubstituteCalls(
            () => plusOne.CallInline(4)
        );

        composed.ToString().Should().Be("() => (4 + 1)");
        var compiled = composed.Compile();
        compiled().Should().Be(5);
    }

    [Fact]
    public void ShouldInlineProperty()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            () => PlusOneProperty.CallInline(4)
        );

        composed.ToString().Should().Be("() => (4 + 1)");
        var compiled = composed.Compile();
        compiled().Should().Be(5);
    }

    [Fact]
    public void ShouldInlineMethodExpression()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            () => GetPlusOneFromMethod().CallInline(4)
        );

        composed.ToString().Should().Be("() => (4 + 1)");
        var compiled = composed.Compile();
        compiled().Should().Be(5);
    }

    [Fact]
    public void ShouldNotInlineWhenInlineFalse()
    {
        var composed = ExpressionComposer.SubstituteCalls(
            (int x) => TIMES_TWO.CallInvoke(x) + 1
        );

        composed.ToString().Should().Be("x => (Invoke(ExposeTests.TIMES_TWO, x) + 1)");
        composed.Compile()(7).Should().Be(15);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Test")]
    private Expression<Func<int, int>> GetPlusOneFromMethod()
    {
        return x => x + 1;
    }
}
