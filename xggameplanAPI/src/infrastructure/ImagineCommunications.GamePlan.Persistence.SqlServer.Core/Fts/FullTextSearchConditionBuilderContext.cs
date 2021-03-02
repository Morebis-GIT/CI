using System.Linq;
using xggameplan.common.Search;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Fts
{
    public class FullTextSearchConditionBuilderContext : SearchConditionBuilderContext<string>
    {
        public override string Build()
        {
            return string.Join(" ", Items.Select(item => item.Build()));
        }
    }
}
