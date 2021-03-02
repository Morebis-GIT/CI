using System.Linq;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices
{
    public class SpotBookingRuleResultChecker : ResultCheckerService<SpotBookingRule>
    {
        private readonly ISpotBookingRuleRepository _spotBookingRuleRepository;

        public SpotBookingRuleResultChecker(ITestDataImporter testDataImporter, ISpotBookingRuleRepository spotBookingRuleRepository) : base(testDataImporter)
        {
            _spotBookingRuleRepository = spotBookingRuleRepository;
        }

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null,
            TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData).ToList();
            var fileTestData = GenerateDataFromFile(fileName, key);

            var spotBookingRules = _spotBookingRuleRepository.GetAll().ToList();
            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                    {
                        featureTestData.AddRange(fileTestData);

                        return spotBookingRules.Count == featureTestData.Count && featureTestData.All(entity => spotBookingRules.Count(c => AreSameSpotBookingRules(c, entity)) == 1);
                    }

                case TestDataResultOperationType.Remove:
                case TestDataResultOperationType.Replace: return true;

                default: return false;
            }
        }

        private bool AreSameSpotBookingRules(SpotBookingRule spotBookingRule1,
            SpotBookingRule spotBookingRule2) =>
            spotBookingRule1.SpotLength == spotBookingRule2.SpotLength &&
            spotBookingRule1.SalesAreas.Count == spotBookingRule2.SalesAreas.Count &&
            spotBookingRule1.MinBreakLength == spotBookingRule2.MinBreakLength &&
            spotBookingRule1.MaxBreakLength == spotBookingRule2.MaxBreakLength &&
            spotBookingRule1.MaxSpots == spotBookingRule2.MaxSpots;
    }
}
