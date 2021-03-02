using System;

namespace xggameplan.common.Search
{
    public abstract class SearchConditionBlockItem<TResult> : SearchConditionItem<TResult>
    {
        protected SearchConditionBlockItem(SearchConditionBuilderContext<TResult> blockBuilderContext)
        {
            BlockBuilderContext = blockBuilderContext ?? throw new ArgumentNullException(nameof(blockBuilderContext));
        }

        public SearchConditionBuilderContext<TResult> BlockBuilderContext { get; }
    }
}
