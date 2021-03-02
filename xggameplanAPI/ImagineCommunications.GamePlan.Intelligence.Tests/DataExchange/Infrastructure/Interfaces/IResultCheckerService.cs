using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.Interfaces
{
    public interface IResultCheckerService
    {
        bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null,
            TestDataResultOperationType operationType = default);
    }
}
