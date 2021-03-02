using System.Linq;
using xggameplan.common.Search;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Fts
{
    public class RegexSearchConditionBuilderContext : SearchConditionBuilderContext<string>
    {
        public override string Build()
        {
            return string.Join(string.Empty, Items.Select(item => item.Build()));
        }
    }
}
