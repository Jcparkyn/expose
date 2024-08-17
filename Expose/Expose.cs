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

    public static Expression<FReturn> Wrap<F1, FReturn>(
        Expression<F1> wrapped,
        Expression<Func<F1, FReturn>> wrapper)
        where F1 : Delegate
        where FReturn : Delegate
    {
        // Extract the parameter from the wrapper, which is the wrapped expression
        var wrappedParameter = wrapper.Parameters[0];

        // Extract the body from the wrapper, which is a func use
        var wrapperBody = wrapper.Body;

        // Substitute the wrapped expression into the wrapper body
        var replacedBody = new ReplaceParameterVisitor(wrappedParameter, wrapped).Visit(wrapperBody) as LambdaExpression;

        // Finally, create a new lambda with the body of the replaced wrapper
        var finalLambda = Expression.Lambda<FReturn>(replacedBody.Body, wrapped.Parameters);

        return finalLambda;
    }

    public static Expression<TReturn> Wrap<F1, F2, TReturn>(
        Expression<F1> wrapped1,
        Expression<F2> wrapped2,
        Expression<Func<F1, F2, TReturn>> wrapper)
    {
        throw new NotImplementedException();
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

    private class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;
        private readonly Expression _replacement;

        public ReplaceParameterVisitor(ParameterExpression parameter, Expression replacement)
        {
            _parameter = parameter;
            _replacement = replacement;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node == _parameter)
                return _replacement;
            return base.VisitParameter(node);
        }
    }
}

public record struct ExposeReturning<TReturn>()
{
    public Expression<Func<T1, TReturn>> Wrap<T1, T2>(
        Expression<Func<T1, T2>> wrapped,
        Expression<Func<
            Func<T1, T2>,
            Expression<Func<T1, TReturn>>
        >> wrapper)
    {
        throw new NotImplementedException();
    }

    public Expression<Func<T1, TReturn>> Wrap0<T1, T2, T3, T4>(
        Expression<Func<T1, T2>> wrapped1,
        Expression<Func<T3, T3>> wrapped2,
        Expression<Func<
            Func<T1, T2>,
            Func<T3, T4>,
            Expression<Func<T1, TReturn>>
        >> wrapper)
    {
        throw new NotImplementedException();
    }

    public Expression<FRet> Wrap1<F1, F2, FRet>(
        Expression<F1> wrapped1,
        Expression<F2> wrapped2,
        Expression<Func<
            F1,
            F2,
            Expression<FRet>
        >> wrapper)
    {
        throw new NotImplementedException();
    }

    public Expression<TReturn> WrapSimple<F1>(
        Expression<F1> wrapped,
        Func<F1, TReturn> wrapper)
    {
        throw new NotImplementedException();
    }

    public Expression<TReturn> WrapSimple<F1, F2>(
        Expression<F1> wrapped1,
        Expression<F2> wrapped2,
        Func<F1, F2, TReturn> wrapper)
    {
        throw new NotImplementedException();
    }
}
