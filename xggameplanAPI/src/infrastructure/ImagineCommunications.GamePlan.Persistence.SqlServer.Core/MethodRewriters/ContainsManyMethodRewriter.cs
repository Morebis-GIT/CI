using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.MethodRewriters
{
    public abstract class ContainsManyMethodRewriter : ExpressionVisitor
    {
        private static readonly MethodInfo _containsMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string) });

        protected override Expression VisitMethodCall(MethodCallExpression node) => TransformMethods(node);

        private string GetMemberValue(object obj, MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return (memberInfo as FieldInfo)?.GetValue(obj)?.ToString();
                case MemberTypes.Property:
                    return (memberInfo as PropertyInfo)?.GetValue(obj)?.ToString();
                default:
                    throw new ArgumentException(
                        $"Can not handle member type {memberInfo.MemberType}, supported only FieldInfo or PropertyInfo MemberType ");
            }
        }

        protected abstract Expression AggregateExpressions(IEnumerable<Expression> expressions);
        
        private string GetContainsArgumentValue(Expression expr)
        {
            var outerMember = expr as MemberExpression;
            var outerExpression = outerMember?.Expression;
            if (outerExpression == null)
            {
                throw  new ArgumentNullException("MemberExpression is null");
            }

            switch (outerExpression.NodeType)
            {
                case ExpressionType.Constant:
                    return GetMemberValue((outerExpression as ConstantExpression)?.Value, outerMember.Member);
                case ExpressionType.MemberAccess:
                {
                    var innerMember = outerExpression as MemberExpression;
                    var innerField = (FieldInfo)innerMember.Member;
                    var ce = innerMember.Expression as ConstantExpression;
                    if (ce == null)
                    {
                        throw new ArgumentException("Not supported Expression. Please consider to use ConstantExpression");
                    }
                    object outerObj = innerField.GetValue(ce.Value);
                    return GetMemberValue(outerObj, outerMember.Member);
                }
                default:
                    throw new ArgumentException("Not supported Expression. Please consider to use ConstantExpression");
            }
        }
        
        private Expression TransformMethods(MethodCallExpression node)
        {
            if (_containsMethodInfo.Equals(node.Method))
            {
                var expressionsList = new List<Expression>();
                
                var val = GetContainsArgumentValue(node.Arguments[0]);
                var searchStrings = val.Split(new []{' '},StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var arg in searchStrings)
                {
                    expressionsList.Add(
                        Expression.Call(
                            node.Object, _containsMethodInfo, Expression.Constant(arg)));
                }

                
                if (expressionsList.Count > 0)
                {
                    return AggregateExpressions(expressionsList);
                }
            }
            return base.VisitMethodCall(node);
        }

        public Expression Rewrite(Expression expression) => Visit(expression);
    }
}
