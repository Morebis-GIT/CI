using System.Linq;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices
{
    public class LengthFactorResultChecker : ResultCheckerService<LengthFactor>
    {
        private ILengthFactorRepository _repository;
        public LengthFactorResultChecker(ITestDataImporter dataImporter, ILengthFactorRepository repository) : base(dataImporter)
        {
            _repository = repository;
        }

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = 0)
        {
            var dataFromTable = GenerateDataFromTable(tableData).ToList();
            var dataFromFile = GenerateDataFromFile(fileName, key).ToList();

            var lengthFactors = _repository.GetAll().ToList();
            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                    {
                        dataFromTable.AddRange(dataFromFile);

                        if (lengthFactors.Count != dataFromTable.Count)
                        {
                            return false;
                        }

                        return dataFromTable.All(entity => lengthFactors.Count(x => AreSame(x, entity)) == 1);
                    }

                case TestDataResultOperationType.Remove:
                case TestDataResultOperationType.Replace:
                    return true;

                default: return false;
            }
        }

        private bool AreSame(LengthFactor obj1, LengthFactor obj2) =>
            obj1.SalesArea == obj2.SalesArea &&
            obj1.Duration == obj2.Duration &&
            obj1.Factor == obj2.Factor;
    }
}
