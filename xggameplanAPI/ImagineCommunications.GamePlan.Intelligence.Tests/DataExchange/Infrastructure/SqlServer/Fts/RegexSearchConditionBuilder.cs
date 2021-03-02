using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using xggameplan.common.Search;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Fts
{
    public class RegexSearchConditionBuilder : SearchConditionBuilderBase<RegexSearchConditionBuilder, string>, IFullTextSearchConditionBuilder
    {
        protected override SearchConditionBuilderContext<string> CreateBuilderContext()
        {
            return new RegexSearchConditionBuilderContext();
        }

        protected override SearchConditionBlockItem<string> CreateBlockItem(SearchConditionBuilderContext<string> blockBuilderContext)
        {
            return new RegexSearchBlockItem(blockBuilderContext);
        }

        public IFollowingActionSearchConditionBuilder<IFullTextSearchConditionBuilder, string> Block(Action<IFullTextSearchConditionBuilder> builder)
        {
            return base.Block(builder);
        }

        public IFollowingActionSearchConditionBuilder<IFullTextSearchConditionBuilder, string> StartWith(string value)
        {
            CurrentBuilderContext.Add(new RegexSearchStartWithItem(value));
            return this;
        }

        public IFollowingActionSearchConditionBuilder<IFullTextSearchConditionBuilder, string> Contains(string value)
        {
            CurrentBuilderContext.Add(new RegexSearchContainsItem(value));
            return this;
        }

        public override string Build()
        {
            return $"^({base.Build()}).*$";
        }
    }
}
