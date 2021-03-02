using System.Linq;
using xggameplan.common.Search;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Fts
{
    public abstract class FullTextSearchItemBase : SearchConditionItem<string>
    {
        protected string LogicalOperandName 
        {
            get
            {
                if (LogicalOperand.HasValue)
                {
                    switch (LogicalOperand)
                    {
                        case SearchLogicalOperand.And:
                            return "AND";
                        case SearchLogicalOperand.AndNot:
                            return "AND NOT";
                        case SearchLogicalOperand.Or:
                            return "OR";
                    }
                }

                return null;
            }
        }

        protected abstract string ConditionTerm { get; }

        public override string Build()
        {
            return string.Join(" ", (new[] {LogicalOperandName, ConditionTerm}).Where(x => x != null));
        }
    }
}
