using System.Collections.Generic;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common
{
    public abstract class ResultCheckerService<TEntity> : IResultCheckerService where TEntity : class
    {
        protected readonly ITestDataImporter TestDataImporter;
        
        protected ResultCheckerService(ITestDataImporter testDataImporter)
        {
            TestDataImporter = testDataImporter;
        }
        
        public abstract bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null,
            TestDataResultOperationType operationType = default);
        
        protected List<TEntity> GenerateDataFromFile(string fileName, string key)
        {
            var result = new List<TEntity>();
            if (fileName is null)
            {
                return result;
            }

            var dataFromFile = TestDataImporter.GetDataFromFile<TEntity>(fileName, key);
            result.AddRange(dataFromFile);
            return result;
        }
        
        protected List<TEntity> GenerateDataFromTable(Table tableData = null)
        {
            var result = new List<TEntity>();
            if (tableData is null)
            {
                return result;
            }

            var dataFromFile = tableData.CreateSet<TEntity>();
            result.AddRange(dataFromFile);
            return result;
        }
    }
}
