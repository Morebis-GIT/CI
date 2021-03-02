using xggameplan.common.Search;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces
{
    public interface IFullTextSearchConditionBuilder : ISearchConditionBuilder<IFullTextSearchConditionBuilder, string>
    {
        IFollowingActionSearchConditionBuilder<IFullTextSearchConditionBuilder, string> StartWith(string value);
        IFollowingActionSearchConditionBuilder<IFullTextSearchConditionBuilder, string> Contains(string value);
    }
}
