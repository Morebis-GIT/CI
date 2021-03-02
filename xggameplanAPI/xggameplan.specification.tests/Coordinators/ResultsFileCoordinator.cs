using System;
using System.Reflection;
using ImagineCommunications.GamePlan.Domain.ResultsFiles;
using TechTalk.SpecFlow;
using xggameplan.specification.tests.Extensions;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Coordinators
{
    [Binding]
    public class ResultsFileCoordinator
    {
        private readonly ILocalFileCache _localFileCache;
        private readonly IResultsFileRepository _resultsFileRepository;

        public ResultsFileCoordinator(ILocalFileCache localFileCache, IResultsFileRepository resultsFileRepository)
        {
            _localFileCache = localFileCache;
            _resultsFileRepository = resultsFileRepository;
        }

        public void CreateLocalFile(string localFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(assembly.GetFullManifestResourceName("ResultFiles.xg_rs_spot.out")))
            {
                _localFileCache.Save(localFileName, stream);
            }
        }

        public void InsertResultFile(Guid scenarioId, string fileId)
        {
            _resultsFileRepository.Insert(scenarioId, fileId, _localFileCache.RootFolder);
        }

        public void GetResultFile(Guid scenarioId, string fileId, bool compressed)
        {
            _localFileCache.Remove(fileId);
            _resultsFileRepository.Get(scenarioId, fileId, compressed, _localFileCache.RootFolder);
        }
    }
}
