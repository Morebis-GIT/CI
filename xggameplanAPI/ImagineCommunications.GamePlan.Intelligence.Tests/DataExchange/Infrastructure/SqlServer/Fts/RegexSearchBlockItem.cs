using xggameplan.common.Search;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Fts
{
    public class RegexSearchBlockItem : SearchConditionBlockItem<string>
    {
        public RegexSearchBlockItem(SearchConditionBuilderContext<string> blockBuilderContext) : base(blockBuilderContext)
        {
        }

        public override string Build()
        {
            return $"({BlockBuilderContext.Build()})";
        }
    }
}
