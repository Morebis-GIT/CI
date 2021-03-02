using System.Linq;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.InventoryStatus
{
    public class InventoryTypeResultChecker : ResultCheckerService<Domain.InventoryStatuses.Objects.InventoryType>
    {
        private readonly IInventoryTypeRepository _inventoryTypeRepository;

        public InventoryTypeResultChecker(ITestDataImporter testDataImporter, IInventoryTypeRepository inventoryTypeRepository) : base(testDataImporter)
        {
            _inventoryTypeRepository = inventoryTypeRepository;
        }

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var inventoryTypes = _inventoryTypeRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return inventoryTypes.Count == featureTestData.Count && featureTestData.All(entity => inventoryTypes.Count(c => c.InventoryCode == entity.InventoryCode) == 1);
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
