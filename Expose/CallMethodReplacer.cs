namespace Expose;

using System.Linq.Expressions;
using System.Reflection;

internal sealed class CallMethodReplacer(bool inline = true) : ExpressionVisitor
{
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.Name == nameof(ExtensionMethods.Call) &&
            node.Method.DeclaringType == typeof(ExtensionMethods))
        {
            var callee = SimplifyCallee(node.Arguments[0]);
            var argument = node.Arguments[1];

            if (inline)
            {
                // Only inline if the callee is a lambda expression
                if (callee is ConstantExpression ce && ce.Value is LambdaExpression lambda)
                {
                    // Replace parameters in the lambda with the argument
                    var replacer = new ParameterReplaceVisitor(lambda.Parameters[0], argument);
                    return Visit(replacer.Visit(lambda.Body));
                }
            }

            // Replace the Call method call with an invocation of the callee
            var invokeExpr = Expression.Invoke(callee, argument);

            return invokeExpr;
        }

        return base.VisitMethodCall(node);
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

    // Helper class to replace a parameter with an argument in an expression tree
    private sealed class ParameterReplaceVisitor(ParameterExpression from, Expression to) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == from ? to : base.VisitParameter(node);
        }
    }
}