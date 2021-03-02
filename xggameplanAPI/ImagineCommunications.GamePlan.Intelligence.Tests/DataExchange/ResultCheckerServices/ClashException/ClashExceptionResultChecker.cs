using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;
using ClashExceptionDomainObject = ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects.ClashException;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.ClashException
{
    public class ClashExceptionResultChecker : ResultCheckerService<ClashExceptionDomainObject>
    {
        private readonly IClashExceptionRepository _clashExceptionRepository;

        public ClashExceptionResultChecker(ITestDataImporter dataImporter, IClashExceptionRepository clashExceptionRepository) : base(dataImporter) =>
            _clashExceptionRepository = clashExceptionRepository;

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var clashExceptions = _clashExceptionRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return clashExceptions.Count == featureTestData.Count && featureTestData.All(entity => clashExceptions.Count(c => AreSameClashExceptions(c, entity)) == 1);
                }
                case TestDataResultOperationType.Remove:
                case TestDataResultOperationType.Replace:
                    return true;
                default:
                    return false;
            }
        }

        private static bool AreSameClashExceptions(ClashExceptionDomainObject ce1, ClashExceptionDomainObject ce2) =>
            ce1.StartDate == ce2.StartDate &&
            ce1.EndDate == ce2.EndDate &&
            ce1.FromType == ce2.FromType &&
            ce1.ToType == ce2.ToType &&
            ce1.IncludeOrExclude == ce2.IncludeOrExclude &&
            ce1.FromValue == ce2.FromValue &&
            ce1.ToValue == ce2.ToValue &&
            ce1.TimeAndDows.Count == ce2.TimeAndDows.Count &&
            ce1.ExternalRef == ce2.ExternalRef;
    }
}
