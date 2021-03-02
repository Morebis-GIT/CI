using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using xggameplan.cloudaccess.Interfaces;
using xggameplan.common.Extensions;
using xggameplan.core.Interfaces;

namespace xggameplan.core.Services.RunCleaning
{
    /// <summary>
    /// Exposes base infrastructure to remove run data from database and S3 bucket.
    /// </summary>
    /// <seealso cref="xggameplan.core.Interfaces.IRunCleaner" />
    public abstract class RunCleanerBase : IRunCleaner
    {
        private readonly ICloudStorageV2 _cloudStorage;

        protected class RunDeletionInfo
        {
            public Guid RunId { get; set; }
            public IReadOnlyCollection<Guid> ScenarioIds { get; set; }
        }

        protected RunCleanerBase(ICloudStorageV2 cloudStorage)
        {
            _cloudStorage = cloudStorage;
        }

        protected abstract Task<IReadOnlyCollection<RunDeletionInfo>> GetRunDeletionInfoAsync(IReadOnlyCollection<Guid> runIds,
            CancellationToken cancellationToken);

        protected abstract Task DeleteRunDataAsync(IReadOnlyCollection<RunDeletionInfo> runDeletionInfos,
            CancellationToken cancellationToken);

        protected virtual Task<(IReadOnlyCollection<string> fileNames, IReadOnlyCollection<string> filePrefixes)>
            PrepareDeletionFileListsAsync(IReadOnlyCollection<RunDeletionInfo> runDeletionInfos,
                CancellationToken cancellationToken)
        {
            var fileNames = new List<string>();
            var filePrefixes = new List<string>();

            foreach (var runDeletionInfo in runDeletionInfos)
            {
                cancellationToken.ThrowIfCancellationRequested();

                fileNames.Add($"input/{runDeletionInfo.RunId}.zip");

                if (runDeletionInfo.ScenarioIds?.Count > 0)
                {
                    fileNames.AddRange(runDeletionInfo.ScenarioIds.Select(scenarioId => $"input/{scenarioId}.zip"));
                    fileNames.AddRange(runDeletionInfo.ScenarioIds.Select(scenarioId => $"output/{scenarioId}.zip"));

                    filePrefixes.AddRange(runDeletionInfo.ScenarioIds.Select(scenarioId => $"reports/{scenarioId}"));
                }
            }

            return Task.FromResult(((IReadOnlyCollection<string>)fileNames, (IReadOnlyCollection<string>)filePrefixes));
        }

        protected virtual async Task DeleteRunFilesAsync(IReadOnlyCollection<RunDeletionInfo> runDeletionInfos,
            CancellationToken cancellationToken)
        {
            var prepareFileListTask = PrepareDeletionFileListsAsync(runDeletionInfos, cancellationToken);
            if (!prepareFileListTask.IsCompleted)
            {
                _ = await prepareFileListTask.ConfigureAwait(false);
            }

            var (fileNames, filePrefixes) = prepareFileListTask.Result;

            // Delete files
            var deleteTask1 = fileNames is null || fileNames.Count == 0
                ? Task.CompletedTask
                : _cloudStorage.DeleteAsync(fileNames, cancellationToken);

            var deleteTask2 = filePrefixes is null || filePrefixes.Count == 0
                ? Task.CompletedTask
                : _cloudStorage.DeleteByPrefixesAsync(filePrefixes, cancellationToken);

            await Task.WhenAll(deleteTask1, deleteTask2).AggregateExceptions().ConfigureAwait(false);
        }

        public virtual Task ExecuteAsync(Guid runId, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(new[] { runId }, cancellationToken);
        }

        public virtual async Task ExecuteAsync(IReadOnlyCollection<Guid> runIds,
            CancellationToken cancellationToken = default)
        {
            if (runIds.Count == 0)
            {
                return;
            }

            var runDeletionInfos =
                await GetRunDeletionInfoAsync(runIds.Count > 1 ? runIds.Distinct().ToArray() : runIds,
                    cancellationToken).ConfigureAwait(false);

            if (runDeletionInfos is null || runDeletionInfos.Count == 0)
            {
                return;
            }

            var runDataTask = DeleteRunDataAsync(runDeletionInfos, cancellationToken);
            var runFilesTask = DeleteRunFilesAsync(runDeletionInfos, cancellationToken);

            await Task.WhenAll(runDataTask, runFilesTask).AggregateExceptions().ConfigureAwait(false);
        }
    }
}
