using xggameplan.common.Search;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Fts
{
    public class FullTextSearchBlockItem : SearchConditionBlockItem<string>
    {
        public FullTextSearchBlockItem(SearchConditionBuilderContext<string> blockBuilderContext) : base(blockBuilderContext)
        {
        }

        public override string Build()
        {
            return $"({BlockBuilderContext.Build()})";
        }
    }
}
