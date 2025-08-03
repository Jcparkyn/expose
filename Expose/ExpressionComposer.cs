namespace Expose;

using System;
using System.Diagnostics;
using System.Linq.Expressions;

/// <summary>
/// Class containing utility methods for composing and exposing <see cref="Expression"/> trees.
/// </summary>
public static class ExpressionComposer
{
    /// <summary>
    /// Replaces calls to the <see cref="ExtensionMethods.CallInline{TRet}(Expression{Func{TRet}})"/>
    /// method with the actual method call in the expression tree.
    /// </summary>
    /// <remarks>
    /// Due to C#'s type inference rules, this overload requires you to explicitly specify the type
    /// of the delegate that <paramref name="value"/> represents, e.g.:
    /// <code>
    ///ExpressionComposer.SubstituteCalls&lt;Func&lt;int, int&gt;&gt;(x =&gt; 1)
    /// </code>
    /// </remarks>
    /// <typeparam name="TDelegate">
    /// The type of the delegate that <paramref name="value"/> represents
    /// </typeparam>
    /// <param name="value">The expression to substitute calls inside of</param>
    /// <returns>
    /// The expression in <paramref name="value"/>, but will all nested expression calls replaced.
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    [DebuggerStepThrough]
    public static Expression<TDelegate> SubstituteCalls<TDelegate>(Expression<TDelegate> value)
        => SubstituteCallsInternal(value);

    /// <inheritdoc cref="SubstituteCalls{TDelegate}(Expression{TDelegate})"/>
    [DebuggerStepThrough]
    public static Expression<Func<T1>> SubstituteCalls<T1>(Expression<Func<T1>> value)
        => SubstituteCallsInternal(value);

    /// <inheritdoc cref="SubstituteCalls{TDelegate}(Expression{TDelegate})"/>
    [DebuggerStepThrough]
    public static Expression<Func<T1, T2>> SubstituteCalls<T1, T2>(Expression<Func<T1, T2>> value)
        => SubstituteCallsInternal(value);

    /// <inheritdoc cref="SubstituteCalls{TDelegate}(Expression{TDelegate})"/>
    [DebuggerStepThrough]
    public static Expression<Func<T1, T2, T3>> SubstituteCalls<T1, T2, T3>(Expression<Func<T1, T2, T3>> value)
        => SubstituteCallsInternal(value);

    /// <inheritdoc cref="SubstituteCalls{TDelegate}(Expression{TDelegate})"/>
    [DebuggerStepThrough]
    public static Expression<Func<T1, T2, T3, T4>> SubstituteCalls<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4>> value)
        => SubstituteCallsInternal(value);

    private static Expression<TDelegate> SubstituteCallsInternal<TDelegate>(Expression<TDelegate> value)
    {
        // Separate method name so we don't accidentally call the same method recursively.
        _ = value ?? throw new ArgumentNullException(nameof(value));
        var visitor = new CallMethodReplacer();
        var newBody = visitor.Visit(value.Body);
        return Expression.Lambda<TDelegate>(newBody, value.Parameters);
    }
}
