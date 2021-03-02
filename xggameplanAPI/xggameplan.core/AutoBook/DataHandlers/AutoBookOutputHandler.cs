using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.AutoBooks.AWS;
using xggameplan.cloudaccess.Business;
using xggameplan.common.Types;
using xggameplan.core.Extensions;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Helpers;
using xggameplan.Extensions;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.Model;
using xggameplan.OutputFiles.Processing;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.RunManagement;

namespace xggameplan.AutoBooks.DataHandlers
{
    /// <summary>
    /// Handles download of output from AWS AutoBook
    /// </summary>
    public class AutoBookOutputHandler : IAutoBookOutputHandler
    {
        private delegate void OutputDataProcessorFunc(IKPICalculationScope scope, IFeatureManager feature, IAuditEventRepository audit, string folder);

        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IScenarioResultRepository _scenarioResultRepository;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly AWSSettings _awsSettings;
        private readonly ICloudStorage _cloudStorage;
        private readonly RootFolder _baseLocalFolder;
        private readonly IPipelineAuditEventRepository _pipelineAuditEventRepository;
        private readonly IFeatureManager _featureManager;
        private readonly IScenarioCampaignMetricsProcessor _scenarioCampaignMetricsProcessor;
        private readonly IKPICalculationScopeFactory _kpiCalculationScopeFactory;
        private readonly bool _includeScenarioPerformanceMeasurementKPIs;

        private static readonly IEnumerable<OutputDataProcessorFunc> _dataProcessingActions =
            new OutputDataProcessorFunc[]
            {
                ProcessData<ScenarioCampaignResult>,
                ProcessData<ScenarioCampaignLevelResult>,
                ProcessData<ProcessBreakEfficiencyOutput>,
                ProcessData<SpotsReqmOutput>,
                ProcessData<ScenarioCampaignFailureOutput>,
                ProcessData<Failures>,
                ProcessData<CampaignsReqmOutput>,
                ProcessData<BaseRatingsOutput>,
                ProcessData<ReserveRatingsOutput>,
                ProcessData<ConversionEfficiencyOutput>
            };

        public AutoBookOutputHandler(AWSSettings awsSettings, ICloudStorage cloudStorage,
            IRepositoryFactory repositoryFactory, IScenarioResultRepository scenarioResultRepository,
            IAuditEventRepository auditEventRepository, RootFolder baseLocalFolder,
            IPipelineAuditEventRepository pipelineAuditEventRepository, IFeatureManager featureManager,
            IScenarioCampaignMetricsProcessor scenarioCampaignMetricsProcessor,
            IKPICalculationScopeFactory kpiCalculationScopeFactory)
        {
            _awsSettings = awsSettings;
            _repositoryFactory = repositoryFactory;
            _scenarioResultRepository = scenarioResultRepository;
            _auditEventRepository = auditEventRepository;
            _cloudStorage = cloudStorage;
            _baseLocalFolder = baseLocalFolder;
            _pipelineAuditEventRepository = pipelineAuditEventRepository;
            _featureManager = featureManager;
            _scenarioCampaignMetricsProcessor = scenarioCampaignMetricsProcessor;
            _kpiCalculationScopeFactory = kpiCalculationScopeFactory;

            _includeScenarioPerformanceMeasurementKPIs = featureManager.IsEnabled(nameof(ProductFeature.ScenarioPerformanceMeasurementKPIs));
        }

        public void Handle(Run run, Guid scenarioId)
        {
            bool cleanup = true;
            try
            {
                // Check for scenario result, shouldn't exist
                ScenarioResult scenarioResult = _scenarioResultRepository.Find(scenarioId);
                if (scenarioResult is null)
                {
                    scenarioResult = new ScenarioResult
                    {
                        Id = scenarioId,
                        TimeCompleted = DateTime.UtcNow
                    };
                }

                // Download output files from AWS
                DownloadOutputFiles(run.Id, scenarioId, GetScenarioLocalFolder(scenarioId));

                // Process output files
                ProcessOutputFiles(run.Id, scenarioId, scenarioResult);
                cleanup = false;
            }
            catch (Exception exception)
            {
                //adding the audit event to clear the warning of 'exception declared but never used' and preserves stack trace with the 'throw'
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, String.Format("Error handling output file for scenario ID: {0}", scenarioId), exception));
                throw;
            }
            finally
            {
                // Delete all output data if it failed
                if (cleanup)
                {
                    try
                    {
                        DeleteOutputData(scenarioId);
                    }
                    catch { };
                }
            }
        }

        /// <summary>
        /// Downloads output files
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="scenarioId"></param>
        /// <param name="remoteFolder"></param>
        /// <param name="localFolder"></param>
        private void DownloadOutputFiles(Guid runId, Guid scenarioId, string localFolder)
        {
            // For some reason the output goes to \Output\[ScenarioId]\output
            string zipFileName = $"{scenarioId}.zip";
            string localZipFile = Path.Combine(localFolder + @"\output", zipFileName);

            try
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineStart(0, 0, PipelineEventIDs.STARTED_DOWNLOADING_ZIP_ARCHIVE_FROM_CLOUD_STORAGE, runId, scenarioId, null, null));
                _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                    PipelineEventIDs.STARTED_DOWNLOADING_ZIP_ARCHIVE_FROM_CLOUD_STORAGE, runId, scenarioId, null));

                // For some reason the output goes to \Output\[ScenarioId]\output
                if (!Directory.Exists(Path.GetDirectoryName(localZipFile)))
                {
                    _ = Directory.CreateDirectory(Path.GetDirectoryName(localZipFile));
                }
                if (File.Exists(localZipFile))
                {
                    File.Delete(localZipFile);
                }

                var filename = $@"output/{zipFileName}";

                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"Downloading scenario output from cloud provider ( Bucket={_awsSettings.S3Bucket},filename={filename} Local Folder={localFolder})"));

                _ = _cloudStorage.Download(new S3DownloadComment
                {
                    BucketName = _awsSettings.S3Bucket,
                    FileName = filename,
                    LocalFileFolder = localFolder
                });

                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0, PipelineEventIDs.FINISHED_DOWNLOADING_ZIP_ARCHIVE_FROM_CLOUD_STORAGE, runId, scenarioId, null, null, null, null));
                _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                    PipelineEventIDs.FINISHED_DOWNLOADING_ZIP_ARCHIVE_FROM_CLOUD_STORAGE, runId, scenarioId, null));
            }
            catch (Exception exception)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0, PipelineEventIDs.FINISHED_DOWNLOADING_ZIP_ARCHIVE_FROM_CLOUD_STORAGE, runId, scenarioId, null, null, exception.Message, exception));
                _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                    PipelineEventIDs.FINISHED_DOWNLOADING_ZIP_ARCHIVE_FROM_CLOUD_STORAGE, runId, scenarioId, exception.Message));

                throw;
            }
            finally
            {
                _pipelineAuditEventRepository.SaveChanges();
            }

            try
            {
                // Clear files in destination folder before we unzip
                ClearFolder(localFolder);
                ZipConversion.DeserializeFromZip(localZipFile, localFolder);
            }
            catch (Exception exception)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, String.Format("Error unzipping {0}", localZipFile), exception));
                throw;
            }
        }

        /// <summary>
        /// Processes the concrete AutoBook output data type.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <param name="scope">Current KPI calculation scope.</param>
        /// <param name="featureManager">The feature manager.</param>
        /// <param name="audit">The audit.</param>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        private static void ProcessData<TData>(IKPICalculationScope scope, IFeatureManager featureManager,
            IAuditEventRepository audit, string folder) where TData : class
        {
            var context = scope.Resolve<IKPICalculationContext>();
            var fileProcessor = scope.Resolve<IOutputFileProcessor<TData>>();

            if (!featureManager.IsServiceEnabled(fileProcessor.GetType()))
            {
                audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"Skipping disabled file processor. ScenarioId: {context.ScenarioId}, FileName: {fileProcessor.FileName}"));
                return;
            }

            var data = fileProcessor.ProcessFile(context.ScenarioId, folder);
            context.SetContextData(data);

            if (scope.TryResolve<IOutputDataHandler<TData>>(out var handler))
            {
                handler.ProcessData(data, context.Snapshot.Run.Value, context.Snapshot.Scenario.Value);
            }
        }

        /// <summary>
        /// Processes files. For each file we pass it to a file processor which
        /// performs some action on it.
        /// </summary>
        /// <param name="runId">RunID</param>
        /// <param name="scenarioId">ScenarioId</param>
        /// <param name="scenarioResult"></param>
        private void ProcessOutputFiles(Guid runId, Guid scenarioId, ScenarioResult scenarioResult)
        {
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineStart(0, 0, PipelineEventIDs.STARTED_EXPORTING_DATA_TO_DATABASE, runId, scenarioId, null, null));
            _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                PipelineEventIDs.STARTED_EXPORTING_DATA_TO_DATABASE, runId, scenarioId, null));

            string scenarioLocalFolder = GetScenarioLocalFolder(scenarioId);
            var dataProcessingExceptions = new List<Exception>();

            using (var kpiCalculationScope = _kpiCalculationScopeFactory.CreateCalculationScope(runId, scenarioId))
            {
                var kpiCalculationContext = kpiCalculationScope.Resolve<IKPICalculationContext>();
                var kpiCalculationManager = kpiCalculationScope.Resolve<IKPICalculationManager>();

                foreach (var dataProcessingAction in _dataProcessingActions)
                {
                    try
                    {
                        dataProcessingAction(kpiCalculationScope, _featureManager, _auditEventRepository, scenarioLocalFolder);
                    }
                    catch (Exception e)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                            $"Error processing: {dataProcessingAction.Method.GetGenericArguments().First().Name}. ScenarioId: {scenarioId}", e));

                        dataProcessingExceptions.Add(e);
                    }
                }

                if (_includeScenarioPerformanceMeasurementKPIs)
                {
                    kpiCalculationContext.SetDefaultKpiDemos();
                }

                _scenarioCampaignMetricsProcessor.ProcessScenarioCampaignMetrics(runId, scenarioId, kpiCalculationContext.Recommendations);

                scenarioResult.Metrics = kpiCalculationManager.CalculateKPIs(runId, scenarioId);
                scenarioResult.AnalysisGroupMetrics = kpiCalculationManager.CalculateAnalysisGroupKPIs(runId);

                _scenarioResultRepository.Add(scenarioResult);
                _scenarioResultRepository.SaveChanges();
            }

            // Exceptions handling
            if (dataProcessingExceptions.Count == 1)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0,
                    PipelineEventIDs.FINISHED_EXPORTING_DATA_TO_DATABASE, runId, scenarioId, null, null,
                    dataProcessingExceptions[0].Message, dataProcessingExceptions[0]));

                throw dataProcessingExceptions[0];
            }
            else if (dataProcessingExceptions.Count > 1)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0,
                    PipelineEventIDs.FINISHED_EXPORTING_DATA_TO_DATABASE, runId, scenarioId, null, null,
                    dataProcessingExceptions[0].Message, dataProcessingExceptions[0]));

                throw new AggregateException(dataProcessingExceptions);
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0,
                PipelineEventIDs.FINISHED_EXPORTING_DATA_TO_DATABASE, runId, scenarioId, null,
                "Import has been finished", null, null));

            _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                PipelineEventIDs.FINISHED_EXPORTING_DATA_TO_DATABASE, runId, scenarioId, null));

            _pipelineAuditEventRepository.SaveChanges();
        }

        /// <summary>
        /// Deletes scenario output data, typically as a result of a failure so
        /// that we don't leave partial data.
        ///
        /// Note that schedule data may have been modified (E.g. Break
        /// efficiency) but we don't reset that because it doesn't relate to a
        /// specific scenario.
        /// </summary>
        /// <param name="scenarioId"></param>
        private void DeleteOutputData(Guid scenarioId)
        {
            var runReset = new RunReset(_repositoryFactory);
            runReset.ResetScenarioOutputData(scenarioId, new List<string>() { "autobook", "isr", "rzr" }, true,
                true, true); // Leave Smooth recommendations, they relate to the whole run
        }

        /// <summary>
        /// Cleans up
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenarioId"></param>
        public void CleanUp(Run run, Guid scenarioId)
        {
            DeleteLocalResultsFiles(GetScenarioLocalFolder(scenarioId));
        }

        private void DeleteLocalResultsFiles(string localFolder)
        {
            if (Directory.Exists(localFolder))
            {
                Directory.Delete(localFolder, true);
            }
        }

        /// <summary>
        /// Clears the contents of the folder, deletes files
        /// </summary>
        /// <param name="localFolder"></param>
        private void ClearFolder(string localFolder)
        {
            if (Directory.Exists(localFolder))
            {
                foreach (string file in Directory.GetFiles(localFolder, "*.*"))
                {
                    File.Delete(file);
                }
            }
        }

        private string GetScenarioLocalFolder(Guid scenarioId) =>
            Path.Combine(_baseLocalFolder, "Output", scenarioId.ToString());
    }
}
