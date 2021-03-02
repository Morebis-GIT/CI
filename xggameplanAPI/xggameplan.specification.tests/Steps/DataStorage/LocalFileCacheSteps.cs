using NUnit.Framework;
using TechTalk.SpecFlow;
using xggameplan.specification.tests.Coordinators;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Steps.DataStorage
{
    [Binding]
    public class LocalFileCacheSteps
    {
        private readonly ResultsFileCoordinator _resultsFileCoordinator;
        private readonly ILocalFileCache _localFileCache;

        public LocalFileCacheSteps(ResultsFileCoordinator resultsFileCoordinator, ILocalFileCache localFileCache)
        {
            _resultsFileCoordinator = resultsFileCoordinator;
            _localFileCache = localFileCache;
        }

        [Given(@"there is '(.*)' result file")]
        public void GivenResultFile(string fileName)
        {
            _resultsFileCoordinator.CreateLocalFile(fileName);
        }

        [Then(@"the result file '(.*)' exists")]
        public void ThenResultFileExists(string fileName)
        {
            Assert.IsTrue(_localFileCache.Exists(fileName), "File '{0}' is expected to be in the local file cache.", fileName);
        }
    }
}
