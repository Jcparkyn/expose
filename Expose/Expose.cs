namespace Expose;

using System.Linq.Expressions;

public static class Expose
{
    public static Func<T1, T2> Func<T1, T2>(Func<T1, T2> f) => f;
    public static Expression<Func<T1, T2>> Expr<T1, T2>(Expression<Func<T1, T2>> f) => f;

    /// <summary>
    /// Returns an expression that is equivalent to <c>x => second(first(x))</c>
    /// </summary>
    public static Expression<Func<T1, T3>> Then<T1, T2, T3>(
        this Expression<Func<T1, T2>> first,
        Expression<Func<T2, T3>> second)
    {
        var firstParameter = first.Parameters[0];
        var firstBody = first.Body;

        var replacedBody = Expression.Invoke(second, firstBody);

        return Expression.Lambda<Func<T1, T3>>(replacedBody, firstParameter);
    }

    public static Expression<Func<T1, T2>> Compose<T1, T2>(Expression<Func<T1, T2>> value)
    {
        var visitor = new CallMethodReplacer();
        var newBody = visitor.Visit(value.Body);
        return Expression.Lambda<Func<T1, T2>>(newBody, value.Parameters);
    }

    public static T2 Call<T1, T2>(this Expression<Func<T1, T2>> expr, T1 arg)
    {
        throw new InvalidOperationException();
    }

    private class CallMethodReplacer : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == nameof(Call) &&
                node.Method.DeclaringType == typeof(Expose))
            {
                var callee = node.Arguments[0];
                var argument = node.Arguments[1];

                // Replace the Call method call with an invocation of the callee
                return Expression.Invoke(callee, argument);
            }

            return base.VisitMethodCall(node);
        }
    }

    private class ReplaceParameterVisitor(ParameterExpression parameter, Expression replacement) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node == parameter)
                return replacement;
            return base.VisitParameter(node);
        }
    }
}
