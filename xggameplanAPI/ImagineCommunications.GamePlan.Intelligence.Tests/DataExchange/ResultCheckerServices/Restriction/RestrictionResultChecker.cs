using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;
using RestrictionDomainObject = ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects.Restriction;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Restriction
{
    public class RestrictionResultChecker : ResultCheckerService<RestrictionDomainObject>
    {
        private readonly IRestrictionRepository _restrictionRepository;

        public RestrictionResultChecker(ITestDataImporter dataImporter, IRestrictionRepository restrictionRepository) : base(dataImporter) =>
            _restrictionRepository = restrictionRepository;

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var restrictions = _restrictionRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return restrictions.Count == featureTestData.Count && featureTestData.All(entity => restrictions.Count(c => c.ExternalIdentifier == entity.ExternalIdentifier) == 1);
                }
                case TestDataResultOperationType.Remove:
                    return true;
                case TestDataResultOperationType.Replace:
                {
                    featureTestData.AddRange(fileTestData);
                    return restrictions.Count == featureTestData.Count && featureTestData.All(entity => restrictions.Count(c => AreSameRestrictions(c, entity)) == 1);
                }
                default:
                    return false;
            }
        }

        private static bool AreSameRestrictions(RestrictionDomainObject source, RestrictionDomainObject target) =>
            source.ExternalIdentifier == target.ExternalIdentifier &&
            source.StartDate.Date == target.StartDate.Date &&
            source.EndDate?.Date == target.EndDate?.Date &&
            source.RestrictionDays == target.RestrictionDays &&
            source.StartTime == target.StartTime &&
            source.EndTime == target.EndTime &&
            source.TimeToleranceMinsAfter == target.TimeToleranceMinsAfter &&
            source.TimeToleranceMinsBefore == target.TimeToleranceMinsBefore &&
            source.SchoolHolidayIndicator == target.SchoolHolidayIndicator &&
            source.PublicHolidayIndicator == target.PublicHolidayIndicator;
    }
}
