namespace Expose;

using System;
using System.Linq.Expressions;

/// <summary>
/// Class containing utility methods for composing and exposing <see cref="Expression"/> trees.
/// </summary>
public static partial class ExpressionComposer
{
    /// <summary>
    /// Replaces calls to the <see cref="ExtensionMethods.Call{TRet}(Expression{Func{TRet}})"/> method with the actual method call in the expression tree.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate that <paramref name="value"/> represents</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Expression<TDelegate> SubstituteCalls<TDelegate>(Expression<TDelegate> value) => SubstituteCallsInternal(value);

    /// <inheritdoc cref="SubstituteCalls{TDelegate}(Expression{TDelegate})"/>
    public static Expression<Func<T1>> SubstituteCalls<T1>(Expression<Func<T1>> value) => SubstituteCallsInternal(value);

    /// <inheritdoc cref="SubstituteCalls{TDelegate}(Expression{TDelegate})"/>
    public static Expression<Func<T1, T2>> SubstituteCalls<T1, T2>(Expression<Func<T1, T2>> value) => SubstituteCallsInternal(value);

    /// <inheritdoc cref="SubstituteCalls{TDelegate}(Expression{TDelegate})"/>
    public static Expression<Func<T1, T2, T3>> SubstituteCalls<T1, T2, T3>(Expression<Func<T1, T2, T3>> value) => SubstituteCallsInternal(value);

    private static Expression<TDelegate> SubstituteCallsInternal<TDelegate>(Expression<TDelegate> value)
    {
        // Separate method name so we don't accidentally call the same method recursively.
        _ = value ?? throw new ArgumentNullException(nameof(value));
        var visitor = new CallMethodReplacer();
        var newBody = visitor.Visit(value.Body);
        return Expression.Lambda<TDelegate>(newBody, value.Parameters);
    }
}
