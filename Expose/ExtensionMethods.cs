namespace Expose;

using System;
using System.Linq.Expressions;

/// <summary>
/// Extensions methods for composing and exposing <see cref="Expression"/> trees.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// A method to represent a call to another expression, from within an expression tree. This
    /// should always be used with <see cref="ExpressionComposer"/> to replace the call with the
    /// actual method invocation.
    /// </summary>
    /// <returns>
    /// Never returns, throws an <see cref="InvalidOperationException"/> if it gets invoked
    /// </returns>
    /// <exception cref="InvalidOperationException">if invoked</exception>
    [ExpressionCallMethod]
    public static TRet Call<TRet>(this Expression<Func<TRet>> expr)
    {
        throw new InvalidOperationException("This method should only be used inside expression trees, and never called");
    }

    /// <inheritdoc cref="Call{TRet}(Expression{Func{TRet}})"/>
    [ExpressionCallMethod]
    public static TRet Call<T1, TRet>(this Expression<Func<T1, TRet>> expr, T1 arg)
    {
        throw new InvalidOperationException("This method should only be used inside expression trees, and never called");
    }

    /// <inheritdoc cref="Call{TRet}(Expression{Func{TRet}})"/>
    [ExpressionCallMethod]
    public static TRet Call<T1, T2, TRet>(this Expression<Func<T1, T2, TRet>> expr, T1 arg1, T2 arg2)
    {
        throw new InvalidOperationException("This method should only be used inside expression trees, and never called");
    }

    /// <inheritdoc cref="Call{TRet}(Expression{Func{TRet}})"/>
    [ExpressionCallMethod]
    public static TRet Call<T1, T2, T3, TRet>(this Expression<Func<T1, T2, T3, TRet>> expr, T1 arg1, T2 arg2, T3 arg3)
    {
        throw new InvalidOperationException("This method should only be used inside expression trees, and never called");
    }

    /// <inheritdoc cref="Call{TRet}(Expression{Func{TRet}})"/>
    [ExpressionCallMethod]
    public static TRet Call<T1, T2, T3, T4, TRet>(this Expression<Func<T1, T2, T3, T4, TRet>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        throw new InvalidOperationException("This method should only be used inside expression trees, and never called");
    }

    /// <summary>
    /// Replaces all calls to the <see
    /// cref="ExtensionMethods.Call{TRet}(Expression{Func{TRet}})">Call(...)</see> method in an <see
    /// cref="IQueryable{T}"/> with the actual expressions.
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
}
