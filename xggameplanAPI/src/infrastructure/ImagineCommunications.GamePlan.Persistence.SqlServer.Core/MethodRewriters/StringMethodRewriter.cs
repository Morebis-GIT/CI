using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.MethodRewriters
{
    // based on the solution from issue https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/996#issuecomment-568416075

    public class StringMethodRewriter : ExpressionVisitor
    {
        private static readonly MethodInfo _startsWithMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string) });

        private static readonly MethodInfo _containsMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string) });

        private static readonly MethodInfo _endsWithMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.EndsWith), new[] { typeof(string) });

        private static readonly MethodInfo _indexOfIngnoreCaseMethodInfo
            = typeof(string).GetRuntimeMethod(
                nameof(string.IndexOf),
                new[] { typeof(string), typeof(StringComparison) });

        private Expression TransformStringMethods(MethodCallExpression node)
        {
            List<Expression> GetArgumentsWithIgnoreCase(IEnumerable<Expression> args)
                => new List<Expression>(node.Arguments) { Expression.Constant(StringComparison.OrdinalIgnoreCase) };

            MethodInfo GetStringMethodWithIgnoreCase(string methodName)
                => typeof(string).GetRuntimeMethod(
                    methodName,
                    new[] { typeof(string), typeof(StringComparison) });

            if (_startsWithMethodInfo.Equals(node.Method) || _endsWithMethodInfo.Equals(node.Method))
            {
                return Expression.Call(
                    node.Object,
                    GetStringMethodWithIgnoreCase(node.Method.Name),
                    GetArgumentsWithIgnoreCase(node.Arguments));
            }

            // Transform Contains to IndexOf >= 0 (.Net Framework issue, fixed in .net core 2 and ^)
            if (_containsMethodInfo.Equals(node.Method))
            {
                var arguments = node.Arguments.ToList();
                arguments.Add(Expression.Constant(StringComparison.OrdinalIgnoreCase));
                return Expression.GreaterThanOrEqual(
                    Expression.Call(
                        node.Object,
                        _indexOfIngnoreCaseMethodInfo,
                        arguments),
                    Expression.Constant(0));
            }
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node) => TransformStringMethods(node);
        public Expression Rewrite(Expression expression) => Visit(expression);

    }
}
