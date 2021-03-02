using System.Linq;
using xggameplan.common.Search;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.Fts
{
    public class RegexSearchConditionBuilderContext : SearchConditionBuilderContext<string>
    {
        public override string Build()
        {
            return string.Join(string.Empty, Items.Select(item => item.Build()));
        }
    }
}
