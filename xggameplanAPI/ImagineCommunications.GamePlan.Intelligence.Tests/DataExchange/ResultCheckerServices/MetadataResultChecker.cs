using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices
{
    public class MetadataResultChecker : ResultCheckerService<Metadata>
    {
        private readonly IMetadataRepository _metadataRepository;

        public MetadataResultChecker(ITestDataImporter testDataImporter, IMetadataRepository metadataRepository)
            : base(testDataImporter)
        {
            _metadataRepository = metadataRepository;
        }

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null,
            TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var metadataFromDb = _metadataRepository.GetAll();
            
            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    var metadataFromFile = featureTestData.First().ToMetadataModel();
                    return AreSameMetadata(metadataFromFile, metadataFromDb);
                }
                case TestDataResultOperationType.Remove:
                case TestDataResultOperationType.Replace:
                    return true;
                default:
                    return false;
            }
        }
        
        private static bool AreSameMetadata(MetadataModel source, MetadataModel target)
        {
            if (source == null && target == null)
            {
                return true;
            }

            if (source == null || target == null || source.Count != target.Count)
            {
                return false;
            }
            
            if (source.Any(entity => target.Count(c => c.Key == entity.Key) != 1))
            {
                return false;
            }

            return source.All(sourceData =>
            {
                var sourceDataItems = sourceData.Value;
                var targetDataItems = target[sourceData.Key];
                
                return sourceDataItems.Count == targetDataItems.Count &&
                       sourceDataItems.All(sourceDataItem => targetDataItems.Count(targetDataItem => AreSameDataItems(sourceDataItem, targetDataItem)) == 1);
            });
        }

        private static bool AreSameDataItems(Data dataItem1, Data dataItem2) =>
            dataItem1.Value.ToString().Equals(dataItem2.Value.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
