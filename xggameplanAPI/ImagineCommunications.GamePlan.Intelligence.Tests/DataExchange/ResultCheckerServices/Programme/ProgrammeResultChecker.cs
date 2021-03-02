using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;
using ProgrammeDbObject = ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects.Programme;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Programme
{
    public class ProgrammeResultChecker : ResultCheckerService<ProgrammeDbObject>
    {
        private readonly IProgrammeRepository _programmeRepository;

        public ProgrammeResultChecker(ITestDataImporter dataImporter, IProgrammeRepository programmeRepository) : base(dataImporter)
        {
            _programmeRepository = programmeRepository;
        }

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var programmes = _programmeRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return programmes.Count == featureTestData.Count && featureTestData.All(entity => programmes.Count(c => AreSameProgrammes(c, entity)) == 1);
                }
                case TestDataResultOperationType.Remove:
                case TestDataResultOperationType.Replace:
                    return true;
                default:
                    return false;
            }
        }

        private static bool AreSameProgrammes(ProgrammeDbObject source, ProgrammeDbObject target) =>
            source.ExternalReference == target.ExternalReference
            && source.ProgrammeName == target.ProgrammeName
            && source.Description == target.Description
            && source.Classification == target.Classification
            && source.LiveBroadcast == target.LiveBroadcast;
    }
}
