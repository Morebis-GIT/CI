using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Extensions;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.ResultsFiles;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;

namespace xggameplan.RunManagement
{
    /// <summary>
    /// Perform reset of run data. For runs then 'reset' typically requires data to be deleted.
    /// </summary>
    internal class RunReset
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public RunReset(IRepositoryFactory repositoryFactory) =>
            _repositoryFactory = repositoryFactory;

        /// <summary>
        /// Resets scenario output data.
        /// </summary>
        public void ResetScenarioOutputData(
            Guid scenarioId,
            IReadOnlyList<string> resetRecommendationProcessors,
            bool resetAutoBookFailures = false,
            bool resetScenarioResults = false,
            bool resetResultFiles = false
            )
        {
            if (resetAutoBookFailures)
            {
                ResetAutoBookFailures(scenarioId);
            }

            if (resetRecommendationProcessors.Count > 0)
            {
                ResetRecommendations(scenarioId, resetRecommendationProcessors);
            }

            if (resetScenarioResults)
            {
                ResetScenarioResults(scenarioId);
            }

            if (resetResultFiles)
            {
                ResetResultFiles(scenarioId);
            }
        }

        private void ResetScenarioResults(Guid scenarioId)
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var scenarioResultRepository = scope.CreateRepository<IScenarioResultRepository>();
                scenarioResultRepository.Remove(scenarioId);
                scenarioResultRepository.SaveChanges();
            }
        }

        private void ResetAutoBookFailures(Guid scenarioId)
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var failuresRepository = scope.CreateRepository<IFailuresRepository>();
                failuresRepository.Delete(scenarioId);
                failuresRepository.SaveChanges();
            }
        }

        private void ResetResultFiles(Guid scenarioId)
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var repositories = scope.CreateRepositories(
                    typeof(IOutputFileRepository),
                    typeof(IResultsFileRepository)
                );
                var outputFileRepository = repositories.Get<IOutputFileRepository>();
                var resultsFileRepository = repositories.Get<IResultsFileRepository>();

                foreach (OutputFile outputFile in outputFileRepository.GetAll())
                {
                    try
                    {
                        if (resultsFileRepository.Exists(scenarioId, outputFile.FileId))
                        {
                            resultsFileRepository.Delete(scenarioId, outputFile.FileId);
                        }
                    }
                    catch { };      // Ignore
                }
            }
        }

        /// <summary>
        /// Resets Smooth failures
        /// </summary>
        /// <param name="runId"></param>
        public void ResetSmoothFailures(Guid runId)
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var smoothFailureRepository = scope.CreateRepository<ISmoothFailureRepository>();
                smoothFailureRepository.RemoveByRunId(runId);
            }
        }

        /// <summary>
        /// Resets recommendations for specified list of processors
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="resetRecommendationProcessors"></param>
        private void ResetRecommendations(Guid scenarioId, IReadOnlyList<string> resetRecommendationProcessors)
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var recommendationRepository = scope.CreateRepository<IRecommendationRepository>();
                recommendationRepository.RemoveByScenarioIdAndProcessors(scenarioId, resetRecommendationProcessors);
                recommendationRepository.SaveChanges();
            }
        }
    }
}
