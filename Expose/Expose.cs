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

    public static Expression<Func<T1, T3>> Wrap<T1, T2, T3>(
        this Expression<Func<T1, T2>> wrapped,
        Expression<Func<
            Func<T1, T2>,
            Expression<Func<T1, T3>>
        >> wrapper)
    {
        throw new NotImplementedException();
    }

    public static Expression<Func<FRet1, FRet2>> WrapSimple<F1, FRet1, FRet2>(
        this Expression<F1> wrapped,
        Func<F1, Func<FRet1, FRet2>> wrapper)
    {
        throw new NotImplementedException();
    }

    public static Expression<Func<T1, bool>> WrapReturningBool<T1, T2>(
        this Expression<Func<T1, T2>> wrapped,
        Expression<Func<
            Func<T1, T2>,
            Expression<Func<T1, bool>>
        >> wrapper)
    {
        throw new NotImplementedException();
    }

    //public static Expression<Func<TInput, bool>> Wrap<TInput, TOutput>(
    //    Expression<Func<TInput, TOutput>> originalExpression,
    //    Expression<Func<
    //        Expression<Func<TInput, TOutput>>,
    //        Expression<Func<TInput, bool>>
    //    >> wrapper)
    //{
    //    throw new NotImplementedException();
    //}

    public static ExposeReturning<T> Returning_<T>() => new();

    public static class Returning<TReturn>
    {
        public static Expression<TReturn> Wrap<F1, F2>(
            (Expression<F1>, Expression<F2>) wrapped,
            Func<F1, F2, TReturn> wrapper)
        {
            throw new NotImplementedException();
        }
    }

    public static class Accepting<TIn>
    {
        public static Expression<Func<TIn, TOut>> Wrap<F1, F2, TOut>(
            (Expression<F1>, Expression<F2>) wrapped,
            Func<F1, F2, Func<TIn, TOut>> wrapper)
        {
            throw new NotImplementedException();
        }
    }

    public static Expression<TReturn> WrapSimple2<F1, F2, TReturn>(
        (Expression<F1>, Expression<F2>) wrapped,
        Expression<Func<F1, F2, TReturn>> wrapper)
    {
        throw new NotImplementedException();
    }

    public static Func<T1, T2> Func<T1, T2>(Func<T1, T2> f) => f;
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
