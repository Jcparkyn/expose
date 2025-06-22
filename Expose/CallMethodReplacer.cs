namespace Expose;

using System.Linq.Expressions;
using System.Reflection;

internal sealed class CallMethodReplacer : ExpressionVisitor
{
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.Name == nameof(ExtensionMethods.Call) &&
            node.Method.DeclaringType == typeof(ExtensionMethods))
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