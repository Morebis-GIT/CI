using System.Linq;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.InventoryStatus
{
    public class LockTypeResultChecker : ResultCheckerService<Domain.InventoryStatuses.Objects.InventoryLockType>
    {
        private readonly ILockTypeRepository _lockTypeRepository;

        public LockTypeResultChecker(ITestDataImporter testDataImporter, ILockTypeRepository lockTypeRepository) : base(testDataImporter)
        {
            _lockTypeRepository = lockTypeRepository;
        }
        
        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var lockTypes = _lockTypeRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return lockTypes.Count == featureTestData.Count && featureTestData.All(entity => lockTypes.Count(c => c.LockType == entity.LockType) == 1);
                }
                case TestDataResultOperationType.Remove:
                case TestDataResultOperationType.Replace:
                    return true;
                default:
                    return false;
            }
        }
    }
}
