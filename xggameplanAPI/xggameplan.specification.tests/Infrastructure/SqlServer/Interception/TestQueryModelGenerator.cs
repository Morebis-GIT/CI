using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Remotion.Linq;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.Interception
{
    public class TestQueryModelGenerator : QueryModelGenerator
    {
        private readonly IFtsInterceptionProvider _ftsInterceptionProvider;

        public TestQueryModelGenerator(INodeTypeProviderFactory nodeTypeProviderFactory,
            IEvaluatableExpressionFilter evaluatableExpressionFilter, ICurrentDbContext currentDbContext,
            IFtsInterceptionProvider ftsInterceptionProvider)
            : base(nodeTypeProviderFactory, evaluatableExpressionFilter, currentDbContext)
        {
            _ftsInterceptionProvider = ftsInterceptionProvider;
        }

        public override QueryModel ParseQuery(Expression query) => base.ParseQuery(ProcessQuery(query));

        protected virtual Expression ProcessQuery(Expression query)
        {
            return new QueryExpressionVisitor(_ftsInterceptionProvider).Visit(query);
        }
    }
}
