using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Campaign
{
    public class CampaignResultChecker : ResultCheckerService<Domain.Campaigns.Objects.Campaign>
    {
        private readonly ICampaignRepository _campaignRepository;

        public CampaignResultChecker(ITestDataImporter dataImporter, ICampaignRepository campaignRepository) : base(dataImporter) =>
            _campaignRepository = campaignRepository;

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var campaigns = _campaignRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return featureTestData.OrderBy(e => e.CustomId)
                        .SequenceEqual(campaigns.OrderBy(e => e.CustomId), new CampaignEqualityComparer());
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
