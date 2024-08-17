using Expose;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Expose.Tests
{
    public class ExposeTests
    {
        [Fact]
        public void Test1()
        {
            Expression<Func<int, string>> first = x => x.ToString();
            var composed = first.Then(x => x + "!");
            var input = 4;
            var output = composed.Compile().Invoke(input);
            output.Should().Be("4!");
        }
    }
}
