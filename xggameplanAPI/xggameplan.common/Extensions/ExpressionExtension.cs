using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace xggameplan.Extensions
{
    public static class ExpressionExtension
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            ParameterExpression p = expr1.Parameters[0];

            var visitor = new SubstExpressionVisitor();
            visitor.subst[expr2.Parameters[0]] = p;

            Expression body = Expression.AndAlso(expr1.Body, visitor.Visit(expr2.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            ParameterExpression p = expr1.Parameters[0];

            var visitor = new SubstExpressionVisitor();
            visitor.subst[expr2.Parameters[0]] = p;

            Expression body = Expression.OrElse(expr1.Body, visitor.Visit(expr2.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> AggregateAnd<T>(this List<Expression<Func<T, bool>>> input)
        {
            return input.Aggregate((l, r) => l.And(r));
        }
    }
}
