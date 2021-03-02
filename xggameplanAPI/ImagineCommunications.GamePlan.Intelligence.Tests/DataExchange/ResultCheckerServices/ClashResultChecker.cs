using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices
{
    public class ClashResultChecker : ResultCheckerService<Clash>
    {
        private readonly IClashRepository _clashRepository;

        public ClashResultChecker(ITestDataImporter dataImporter, IClashRepository clashRepository) : base(dataImporter)
        {
            _clashRepository = clashRepository;

        }

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null,
            TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var allClashes = _clashRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                    {
                        featureTestData.AddRange(fileTestData);
                        if (allClashes.Count != featureTestData.Count)
                        {
                            return false;
                        }

                        foreach (var entity in featureTestData)
                        {
                            if (allClashes.Count(c => c.Externalref == entity.Externalref) != 1)
                            {
                                return false;
                            }

                            var storedClash = allClashes.FirstOrDefault(c => c.Externalref == entity.Externalref);
                            if (!CompareClash(entity, storedClash))
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

        public bool CompareClash(Clash source, Clash target) =>
            source.Externalref == target.Externalref &&
            source.Description == target.Description &&
            source.ExposureCount == target.ExposureCount &&
            source.ParentExternalidentifier == target.ParentExternalidentifier;
    }
}
