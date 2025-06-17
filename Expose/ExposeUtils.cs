namespace Expose;

using System;
using System.Linq.Expressions;
using System.Reflection;

/// <summary>
/// Class containing utility methods for composing and exposing <see cref="Expression"/> trees.
/// </summary>
public static class ExposeUtils
{
    /// <summary>
    /// Returns an expression that is equivalent to <c>x => second(first(x))</c>
    /// </summary>
    public static Expression<Func<T1, T3>> Then<T1, T2, T3>(
        this Expression<Func<T1, T2>> first,
        Expression<Func<T2, T3>> second)
    {
        _ = first ?? throw new ArgumentNullException(nameof(first));
        var firstParameter = first.Parameters[0];
        var firstBody = first.Body;

        var replacedBody = Expression.Invoke(second, firstBody);

        return Expression.Lambda<Func<T1, T3>>(replacedBody, firstParameter);
    }

    /// <summary>
    /// Replaces calls to the <see cref="Call{TRet}(Expression{Func{TRet}})"/> method with the actual method call in the expression tree.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate that <paramref name="value"/> represents</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Expression<TDelegate> SubstituteCalls<TDelegate>(Expression<TDelegate> value)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));
        var visitor = new CallMethodReplacer();
        var newBody = visitor.Visit(value.Body);
        return Expression.Lambda<TDelegate>(newBody, value.Parameters);
    }

    /// <inheritdoc cref="SubstituteCalls{TDelegate}(Expression{TDelegate})"/>
    public static Expression<Func<T1>> SubstituteCalls<T1>(Expression<Func<T1>> value) => SubstituteCalls(value);

    /// <inheritdoc cref="SubstituteCalls{TDelegate}(Expression{TDelegate})"/>
    public static Expression<Func<T1, T2>> SubstituteCalls<T1, T2>(Expression<Func<T1, T2>> value) => SubstituteCalls(value);

    /// <inheritdoc cref="SubstituteCalls{TDelegate}(Expression{TDelegate})"/>
    public static Expression<Func<T1, T2, T3>> SubstituteCalls<T1, T2, T3>(Expression<Func<T1, T2, T3>> value) => SubstituteCalls(value);

    /// <summary>
    /// A method to represent a call to another expression, from within an expression tree.
    /// This should always be used with <see cref="SubstituteCalls{TDelegate}(Expression{TDelegate})"/> to replace the call with the actual method invocation."
    /// </summary>
    /// <returns>Never returns, throws an <see cref="InvalidOperationException"/> if it gets invoked</returns>
    /// <exception cref="InvalidOperationException">if invoked</exception>
    public static TRet Call<TRet>(this Expression<Func<TRet>> expr)
    {
        throw new InvalidOperationException("This method should only be used inside expression trees, and never called");
    }

    /// <inheritdoc cref="Call{TRet}(Expression{Func{TRet}})"/>
    public static TRet Call<T1, TRet>(this Expression<Func<T1, TRet>> expr, T1 arg)
    {
        throw new InvalidOperationException("This method should only be used inside expression trees, and never called");
    }

    /// <inheritdoc cref="Call{TRet}(Expression{Func{TRet}})"/>
    public static TRet Call<T1, T2, TRet>(this Expression<Func<T1, T2, TRet>> expr, T1 arg1, T2 arg2)
    {
        throw new InvalidOperationException("This method should only be used inside expression trees, and never called");
    }

    /// <summary>
    /// Replaces all calls to the <see cref="Call{TRet}(Expression{Func{TRet}})">Call(...)</see> method in an <see cref="IQueryable{T}"/> with the actual expressions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IQueryable<T> SubstituteCalls<T>(this IQueryable<T> self)
    {
        _ = self ?? throw new ArgumentNullException(nameof(self));
        var visitor = new CallMethodReplacer();
        var newExpression = visitor.Visit(self.Expression);
        return self.Provider.CreateQuery<T>(newExpression);
    }

    private sealed class CallMethodReplacer : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == nameof(Call) &&
                node.Method.DeclaringType == typeof(ExposeUtils))
            {
                var callee = node.Arguments[0];
                var argument = node.Arguments[1];

                // Replace the Call method call with an invocation of the callee
                return Expression.Invoke(SimplifyCallee(callee), argument);
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return base.VisitLambda(node);
        }

        /// <summary>
        /// Simplifies the callee expression if it's a reference to a constant field or property.
        /// </summary>
        private static Expression SimplifyCallee(Expression callee)
        {
            if (callee is MemberExpression
                {
                    Expression: ConstantExpression { Value: { } constant },
                    Member: var member
                })
            {
                return member switch
                {
                    FieldInfo fi => Expression.Constant(fi.GetValue(constant), callee.Type),
                    PropertyInfo pi => Expression.Constant(pi.GetValue(constant), callee.Type),
                    _ => callee
                };
            }
            return callee;
        }
    }
}
