using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using xggameplan.common.Search;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Fts
{
    public class FullTextSearchConditionBuilder : SearchConditionBuilderBase<FullTextSearchConditionBuilder, string>, IFullTextSearchConditionBuilder
    {
        protected override SearchConditionBuilderContext<string> CreateBuilderContext()
        {
            return new FullTextSearchConditionBuilderContext();
        }

        protected override SearchConditionBlockItem<string> CreateBlockItem(SearchConditionBuilderContext<string> blockBuilderContext)
        {
            return new FullTextSearchBlockItem(blockBuilderContext);
        }

        public IFollowingActionSearchConditionBuilder<IFullTextSearchConditionBuilder, string> StartWith(string value)
        {
            CurrentBuilderContext.Add(new FullTextSearchStartWithItem(value));
            return this;
        }

        public IFollowingActionSearchConditionBuilder<IFullTextSearchConditionBuilder, string> Contains(string value)
        {
            CurrentBuilderContext.Add(new FullTextSearchContainsItem(value));
            return this;
        }

        public IFollowingActionSearchConditionBuilder<IFullTextSearchConditionBuilder, string> Block(Action<IFullTextSearchConditionBuilder> builder)
        {
            return base.Block(builder);
        }
    }
}
