using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
	
using xggameplan.common.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.Interception
{
    internal class QueryExpressionVisitor : ExpressionVisitor
    {
        private static readonly MethodInfo StringIndexOf =
            ReflectionExtensions.GetMethodInfo<string>(s =>
                s.IndexOf(string.Empty, StringComparison.OrdinalIgnoreCase));
        private static readonly MethodInfo InvariantStringEquals = typeof(string)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .FirstOrDefault(m => m.Name == nameof(string.Equals) && m.GetParameters().Length == 3);
        private static readonly MethodInfo InvariantEnumerableContains = typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .FirstOrDefault(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 3)
            ?.MakeGenericMethod(typeof(string));

        private static readonly MethodInfo Contains3Args = typeof(DbInterceptedFunctions)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .FirstOrDefault(m => m.Name == nameof(DbInterceptedFunctions.Contains) && m.GetParameters().Length == 4);
        private static readonly MethodInfo Contains4Args = typeof(DbInterceptedFunctions)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .FirstOrDefault(m => m.Name == nameof(DbInterceptedFunctions.Contains) && m.GetParameters().Length == 5);

        private readonly IFtsInterceptionProvider _ftsInterceptionProvider;

        internal QueryExpressionVisitor(IFtsInterceptionProvider ftsInterceptionProvider)
        {
            _ftsInterceptionProvider = ftsInterceptionProvider;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            //string arg1 == string arg2
            if (node.NodeType == ExpressionType.Equal && node.Left.Type == typeof(string) && node.Right.Type == typeof(string))
            {
                //replaces to string.Equals(arg1, arg2, StringComparison.OrdinalIgnoreCase)
                return Expression.Call(InvariantStringEquals, node.Left, node.Right,
                    Expression.Constant(StringComparison.OrdinalIgnoreCase));
            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == nameof(DbFunctionsExtensions.Contains) &&
                node.Method.DeclaringType == typeof(DbFunctionsExtensions))
            {
                MethodInfo methodInfo = null;
                if (node.Arguments.Count == 3)
                {
                    methodInfo = Contains3Args;
                }
                else if (node.Arguments.Count == 4)
                {
                    methodInfo = Contains4Args;
                }

                if (methodInfo != null)
                {
                    var descriptor = new FtsContainsMethodDescriptor();
                    _ = new FtsPropertyReferenceVisitor(descriptor).Visit(node.Arguments[1]);

                    var methodParams = new List<Expression>
                    {
                        Expression.Constant(_ftsInterceptionProvider),
                        descriptor.EntityReference,
                        descriptor.PropertyReference,
                    };
                    methodParams.AddRange(node.Arguments.Skip(2));

                    return Expression.Call(methodInfo, methodParams.ToArray());
                }
            }
            else
            //List<string>.Contains(arg1)
            if (node.Method.Name == nameof(List<string>.Contains) &&
                node.Method.DeclaringType == typeof(List<string>) && node.Object != null)
            {
                //replaces to Enumerable.Contains(List<string>, arg1, StringComparer.OrdinalIgnoreCase)
                return Expression.Call(InvariantEnumerableContains,
                    node.Object,
                    node.Arguments.First(),
                    Expression.Constant(StringComparer.OrdinalIgnoreCase));
            }
            else
            //IEnumerable<string>.Contains(arg1)
            if (node.Method.Name == nameof(Enumerable.Contains) &&
                node.Method.DeclaringType == typeof(Enumerable) && node.Arguments.Count == 2 &&
                typeof(IEnumerable<string>).IsAssignableFrom(node.Arguments[0].Type))
            {
                //replaces to Enumerable.Contains(IEnumerable<string>, arg1, StringComparer.OrdinalIgnoreCase)
                var args = new List<Expression>(node.Arguments) {Expression.Constant(StringComparer.OrdinalIgnoreCase)};
                return Expression.Call(InvariantEnumerableContains, args);
            }
            else
            //string.Contains(arg1)
            if (node.Method.Name == nameof(string.Contains) && node.Method.DeclaringType == typeof(string) &&
                node.Object != null)
            {
                //replaces to string.IndexOf(arg1, StringComparison.OrdinalIgnoreCase) >= 0
                return Expression.GreaterThanOrEqual(
                    Expression.Call(node.Object, StringIndexOf, node.Arguments[0],
                        Expression.Constant(StringComparison.OrdinalIgnoreCase)),
                    Expression.Constant(0));
            }

            return base.VisitMethodCall(node);
        }
    }
}
