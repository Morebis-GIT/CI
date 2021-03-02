using System;
using TechTalk.SpecFlow;
using xggameplan.specification.tests.Coordinators;
using xggameplan.specification.tests.Extensions;

namespace xggameplan.specification.tests.Steps.DataStorage
{
    [Binding]
    public class ResultsFileRepositorySteps
    {
        private readonly ResultsFileCoordinator _resultsFileCoordinator;

        public ResultsFileRepositorySteps(ResultsFileCoordinator resultsFileCoordinator)
        {
            _resultsFileCoordinator = resultsFileCoordinator;
        }

        [Given(@"there is result file '(.*)' for scenario '(.*)'")]
        public void GivenThereIsResultFile(string fileId, string scenarioId)
        {
            _resultsFileCoordinator.CreateLocalFile(fileId);
            _resultsFileCoordinator.InsertResultFile(scenarioId.SpecflowConvert<Guid>(), fileId);
        }
    }
}
