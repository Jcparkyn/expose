namespace Expose;

using System;
using System.Linq.Expressions;

/// <summary>
/// If applied to a method, indicates that the method is intended to be used as a call within an expression tree, for <see cref="ExpressionComposer"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ExpressionCallMethodAttribute(bool inline = true) : Attribute
{
    /// <summary>
    /// If true, the method body will be inlined into the expression tree when it is substituted.
    /// Otherwise, the method will be replaced with an <see cref="InvocationExpression"/> that calls the method.
    /// </summary>
    public bool Inline => inline;
}
