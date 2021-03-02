using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.SalesArea
{
    public class SalesAreaResultChecker : ResultCheckerService<Domain.Shared.SalesAreas.SalesArea>
    {
        private readonly ISalesAreaRepository _salesAreaRepository;

        public SalesAreaResultChecker(ITestDataImporter dataImporter, ISalesAreaRepository salesAreaRepository) : base(dataImporter) =>
            _salesAreaRepository = salesAreaRepository;

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var salesAreas = _salesAreaRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                    {
                        featureTestData.AddRange(fileTestData);

                        if (salesAreas.Count != featureTestData.Count)
                        {
                            return false;
                        }
                        foreach (var salesArea in salesAreas)
                        {
                            if (featureTestData.Count(target => CompareSalesArea(salesArea, target)) != 1)
                            {
                                return false;
                            }
                        }
                        return true;

                    }
                default:
                    return false;
            }
        }

        private static bool CompareSalesArea(Domain.Shared.SalesAreas.SalesArea source, Domain.Shared.SalesAreas.SalesArea target) =>
            source.Name == target.Name &&
            source.ShortName == target.ShortName &&
            source.TargetAreaName == target.TargetAreaName &&
            source.BaseDemographic1 == target.BaseDemographic1 &&
            source.BaseDemographic2 == target.BaseDemographic2 &&
            source.StartOffset == target.StartOffset &&
            source.DayDuration == target.DayDuration &&
            source.CustomId == target.CustomId;
    }
}
