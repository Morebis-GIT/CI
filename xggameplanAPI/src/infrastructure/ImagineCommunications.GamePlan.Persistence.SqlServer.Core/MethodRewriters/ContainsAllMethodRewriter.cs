using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.MethodRewriters
{
    public class ContainsAllMethodRewriter : ContainsManyMethodRewriter
    {
        protected override Expression AggregateExpressions(IEnumerable<Expression> expressions) => expressions.Aggregate((l, r) => Expression.And(l, r));
    }
}
