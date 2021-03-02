using System.Linq;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;
using InventoryLockDbObject = ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects.InventoryLock;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.InventoryStatus
{
    public class InventoryLockResultChecker : ResultCheckerService<InventoryLockDbObject>
    {
        private readonly IInventoryLockRepository _inventoryLockRepository;

        public InventoryLockResultChecker(ITestDataImporter testDataImporter, IInventoryLockRepository inventoryLockRepository) : base(testDataImporter)
        {
            _inventoryLockRepository = inventoryLockRepository;
        }

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var inventoryLocks = _inventoryLockRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return inventoryLocks.Count == featureTestData.Count && featureTestData.All(entity => inventoryLocks.Count(c => AreSameInventoryLocks(c, entity)) == 1);
                }
                case TestDataResultOperationType.Remove:
                case TestDataResultOperationType.Replace:
                    return true;
                default:
                    return false;
            }
        }

        private static bool AreSameInventoryLocks(InventoryLockDbObject source, InventoryLockDbObject target) =>
            source.InventoryCode == target.InventoryCode &&
            source.SalesArea == target.SalesArea &&
            source.StartDate == target.StartDate &&
            source.EndDate == target.EndDate &&
            source.StartTime == target.StartTime &&
            source.EndTime == target.EndTime;
    }
}
