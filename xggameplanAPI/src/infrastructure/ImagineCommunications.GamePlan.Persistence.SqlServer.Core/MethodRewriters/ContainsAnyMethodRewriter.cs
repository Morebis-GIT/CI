using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.MethodRewriters
{
    public class ContainsAnyMethodRewriter : ContainsManyMethodRewriter
    {
        protected override Expression AggregateExpressions(IEnumerable<Expression> expressions) =>
            expressions.Aggregate((l, r) => Expression.Or(l, r));
    }
}
