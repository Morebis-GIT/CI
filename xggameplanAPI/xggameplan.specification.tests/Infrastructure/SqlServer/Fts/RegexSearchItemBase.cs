using System.Linq;
using xggameplan.common.Search;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.Fts
{
    public abstract class RegexSearchItemBase : SearchConditionItem<string>
    {
        protected string GetRegexGroup(string regExExpression)
        {
            var lookAhead = LogicalOperand == SearchLogicalOperand.AndNot ? "!" : "=";
            return $"(?{lookAhead}.*{regExExpression})";
        }

        protected abstract string ConditionExpression { get; }

        public override string Build()
        {
            return string.IsNullOrEmpty(ConditionExpression)
                ? string.Empty
                : string.Join(string.Empty,
                    (new[] {LogicalOperand == SearchLogicalOperand.Or ? "|" : null, GetRegexGroup(ConditionExpression)})
                    .Where(x => x != null));
        }
    }
}
