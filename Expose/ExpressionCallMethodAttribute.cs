namespace Expose;

using System;

/// <summary>
/// If applied to a method, indicates that the method is intended to be used as a call within an expression tree, for <see cref="ExpressionComposer"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ExpressionCallMethodAttribute : Attribute
{
}
