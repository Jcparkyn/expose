namespace Expose;

using System.Data;
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
            var arguments = node.Arguments.Skip(1);

            if (inline)
            {
                var lambda = callee switch
                {
                    ConstantExpression { Value: LambdaExpression l } => l,
                    _ => Expression.Lambda<Func<LambdaExpression>>(callee).Compile()()
                };
                // Replace parameters in the lambda with the arguments
                var paramDict = arguments
                    .Zip(lambda.Parameters, (arg, param) => (arg, param))
                    .ToDictionary(t => t.param, t => t.arg);
                var replacer = new ParameterReplaceVisitor(paramDict);
                return Visit(replacer.Visit(lambda.Body));
            }

            // Replace the Call method call with an invocation of the callee
            var invokeExpr = Expression.Invoke(callee, arguments);

            return invokeExpr;
        }

        return base.VisitMethodCall(node);
    }

    /// <summary>
    /// Simplifies the callee expression if it's a reference to a constant field or property.
    /// </summary>
    private static Expression SimplifyCallee(Expression callee)
    {
        if (callee is MemberExpression me)
        {
            switch (me.Expression, me.Member)
            {
                case (ConstantExpression ce, FieldInfo fi):
                    return Expression.Constant(fi.GetValue(ce.Value), callee.Type);
                case (ConstantExpression ce, PropertyInfo pi):
                    return Expression.Constant(pi.GetValue(ce.Value), callee.Type);
                case (null, FieldInfo fi) when fi.IsStatic:
                    return Expression.Constant(fi.GetValue(null), callee.Type);
                case (null, PropertyInfo pi):
                    return Expression.Constant(pi.GetValue(null), callee.Type);
            }
        }
        return callee;
    }

    // Helper class to replace a parameter with an argument in an expression tree
    private sealed class ParameterReplaceVisitor(Dictionary<ParameterExpression, Expression> substitutions)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return substitutions.TryGetValue(node, out var replacement)
                ? replacement
                : base.VisitParameter(node);
        }
    }
}