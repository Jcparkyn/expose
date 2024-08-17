using Expose;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xunit;

namespace Expose.Tests
{
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
        public void Wrap()
        {
            Expression<Func<int, int>> addOne = x => x + 1;
            //var wrapped = Expose.Wrap<int, int, bool>(addOne, addOne =>
            //    x => addOne(x) == 3
            //);
            var wrapped = Expose.Returning_<bool>().Wrap(addOne, addOne =>
                x => addOne(x) == 3
            );
            var output1 = wrapped.Compile().Invoke(1);
            output1.Should().BeFalse();
            var output2 = wrapped.Compile().Invoke(2);
            output2.Should().BeTrue();
        }

        [Fact]
        public void Wrap2()
        {
            Expression<Func<int, bool>> isNegative = x => x < 0;
            Expression<Func<int, int>> mod2 = x => x % 2;
            var wrapped = Expose.Returning<Func<int, bool>>.Wrap(
                (isNegative, mod2),
                (isNegative, mod2) =>
                    (int x) => isNegative(x) || mod2(x) == 1
            );

            var wrapped2 = Expose.Accepting<int>.Wrap(
                (isNegative, mod2),
                (isNegative, mod2) =>
                    (Func<int, bool>)(x => isNegative(x) || mod2(x) == 1)
            );

            var wrapped3 = Expose.WrapSimple2(
                (isNegative, mod2),
                (isNegative, mod2) => Expose.Func(
                    (int x) => isNegative(x) || mod2(x) == 1
                )
            );
        }
    }
}
