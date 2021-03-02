using System;

namespace xggameplan.common.Search
{
    public interface ISearchConditionBuilder<out TBuilder, out TResult> : IBuildAction<TResult>
        //where TBuilder : ISearchConditionBuilder<TBuilder, TResult>
    {
        IFollowingActionSearchConditionBuilder<TBuilder, TResult> Block(Action<TBuilder> builder);
    }

    public interface IFollowingActionSearchConditionBuilder<out TBuilder, out TResult> : IBuildAction<TResult>
    {
        TBuilder And();
        TBuilder AndNot();
        TBuilder Or();
    }
}
