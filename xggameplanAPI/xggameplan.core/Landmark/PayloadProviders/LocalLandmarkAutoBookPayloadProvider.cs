using System;
using ImagineCommunications.GamePlan.Domain.Runs;
using xggameplan.common.Types;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.core.Services;

namespace xggameplan.core.Landmark.PayloadProviders
{
    public class LocalLandmarkAutoBookPayloadProvider : LandmarkAutoBookPayloadProviderBase
    {
        private readonly OptimiserInputFiles _inputFilesGenerator;
        private readonly IRunRepository _runRepository;

        public LocalLandmarkAutoBookPayloadProvider(OptimiserInputFiles inputFilesGenerator, IRunRepository runRepository, RootFolder baseLocalFolder) : base(baseLocalFolder)
        {
            _inputFilesGenerator = inputFilesGenerator;
            _runRepository = runRepository;
        }

        protected override string DownloadScenarioInputFiles(Guid scenarioId, string localFolder, string localInputFolder)
        {
            var run = _runRepository.FindByScenarioId(scenarioId);

            if (run is null)
            {
                throw new Exception($"Run for scenario: {scenarioId.ToString()} was not found");
            }

            return _inputFilesGenerator.PopulateScenarioData(run, scenarioId);
        }

        protected override string DownloadRunInputFiles(Guid runId, string localFolder, string localInputFolder)
        {
            var run = _runRepository.Find(runId);

            if (run is null)
            {
                throw new Exception($"Run {runId.ToString()} was not found");
            }

            return _inputFilesGenerator.PopulateRunData(run);
        }
    }
}
