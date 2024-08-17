namespace Expose;

using System.Linq.Expressions;

public static class Expose
{
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

    public static Expression<?> Wrap<>(
        )
}
