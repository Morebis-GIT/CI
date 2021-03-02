using System;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.AutoBooks.AWS;
using xggameplan.cloudaccess.Business;
using xggameplan.common.Services;
using xggameplan.core.Helpers;
using xggameplan.core.Services;
using xggameplan.Model;

namespace xggameplan.AutoBooks.DataHandlers
{
    /// <summary>
    /// Handles uploading of input to AWS AutoBook
    /// </summary>
    internal class AutoBookInputHandler : IAutoBookInputHandler
    {
        private readonly OptimiserInputFiles _optimiserInputFiles;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly AWSSettings _awsSettings;
        private readonly ICloudStorage _cloudStorage;
        private readonly IPipelineAuditEventRepository _pipelineAuditEventRepository;

        public AutoBookInputHandler(OptimiserInputFiles optimiserInputFiles, IAuditEventRepository auditEventRepository,
            AWSSettings awsSettings, ICloudStorage cloudStorage, IPipelineAuditEventRepository pipelineAuditEventRepository)
        {
            _optimiserInputFiles = optimiserInputFiles;
            _auditEventRepository = auditEventRepository;
            _awsSettings = awsSettings;
            _cloudStorage = cloudStorage;
            _pipelineAuditEventRepository = pipelineAuditEventRepository;
        }

        public void Handle(Run run, Guid scenarioId)
        {
            UploadRunData(run, scenarioId);
        }

        private void UploadRunData(Run run, Guid scenarioId)
        {
            string runFilePath = null;
            string scenarioFilePath = null;
            bool uploadRunData = false;
            bool uploadScenarioData = false;
            string runFile = $"{run.Id}.zip";
            string runFileNameWithPath = $@"input/{runFile}";
            string scenarioFile = $"{scenarioId}.zip";
            string scenarioFileNameWithPath = $@"input/{scenarioFile}";
            bool loggedStarted = false;

            try
            {
                // Ensure that only one instance attempts to upload <RunId>.zip. We can't just check FileExists because it will take time for the file to
                // be uploaded and appear.
                using (MachineLock.Create($"xggameplan.AWSInputHandler.UploadRunData.Run Id: {run.Id}", new TimeSpan(2, 0, 0)))
                {
                    if (!_cloudStorage.FileExists(new S3FileComment()
                    {
                        BucketName = _awsSettings.S3Bucket,
                        FileNameWithPath = runFileNameWithPath
                    }))
                    {
                        uploadRunData = true;
                        if (!loggedStarted)
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineStart(
                                0,
                                0, PipelineEventIDs.STARTED_GENERATING_INPUT_FILES, run.Id, scenarioId, null,
                                null));

                            _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                                PipelineEventIDs.STARTED_GENERATING_INPUT_FILES, run.Id, scenarioId, null));

                            loggedStarted = true;
                        }

                        runFilePath = _optimiserInputFiles.PopulateRunData(run);
                    }
                }

                // Upload scenario data
                if (!_cloudStorage.FileExists(new S3FileComment() { BucketName = _awsSettings.S3Bucket, FileNameWithPath = scenarioFileNameWithPath }))
                {
                    uploadScenarioData = true;
                    if (!loggedStarted)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineStart(0, 0,
                            PipelineEventIDs.STARTED_GENERATING_INPUT_FILES, run.Id, scenarioId, null, null));
                        _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                            PipelineEventIDs.STARTED_GENERATING_INPUT_FILES, run.Id, scenarioId, null));
                        loggedStarted = true;
                    }

                    scenarioFilePath = _optimiserInputFiles.PopulateScenarioData(run, scenarioId);
                }

                if (loggedStarted)
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0, PipelineEventIDs.FINISHED_GENERATING_INPUT_FILES, run.Id, scenarioId, null, null, null, null));
                    _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                        PipelineEventIDs.FINISHED_GENERATING_INPUT_FILES, run.Id, scenarioId, null));
                }
            }
            catch (System.Exception exception)
            {
                if (loggedStarted)
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0, PipelineEventIDs.FINISHED_GENERATING_INPUT_FILES, run.Id, scenarioId, null, null, exception.Message, exception));
                    _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                        PipelineEventIDs.FINISHED_GENERATING_INPUT_FILES, run.Id, scenarioId, exception.Message));
                }
                throw;
            }
            finally
            {
                _pipelineAuditEventRepository.SaveChanges();
            }

            // Because we do zipping as we create each input file...
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineStart(0, 0, PipelineEventIDs.STARTED_ZIPPING_INPUT_FILES, run.Id, scenarioId, null, null));
            _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                PipelineEventIDs.STARTED_ZIPPING_INPUT_FILES, run.Id, scenarioId, null));
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0, PipelineEventIDs.FINISHED_ZIPPING_INPUT_FILES, run.Id, scenarioId, null, null, null, null));
            _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                PipelineEventIDs.FINISHED_ZIPPING_INPUT_FILES, run.Id, scenarioId, null));

            _pipelineAuditEventRepository.SaveChanges();

            try
            {
                using (MachineLock.Create(
                    $"xggameplan.AWSInputHandler.UploadRunData.Run Id: {run.Id}", new TimeSpan(2, 0, 0)))
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineStart(0, 0,
                        PipelineEventIDs.STARTED_UPLOADING_INPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE, run.Id, scenarioId, null,
                        null));
                    _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                        PipelineEventIDs.STARTED_UPLOADING_INPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE, run.Id, scenarioId, null));

                    //Upload run data
                    if (uploadRunData && !String.IsNullOrWhiteSpace(runFilePath) && File.Exists(runFilePath) && !_cloudStorage.FileExists(new S3FileComment()
                    {
                        BucketName = _awsSettings.S3Bucket,
                        FileNameWithPath = runFileNameWithPath
                    }))
                    {
                        _cloudStorage.Upload(new S3UploadComment()
                        {
                            BucketName = _awsSettings.S3Bucket,
                            DestinationFilePath = runFileNameWithPath,
                            SourceFilePath = runFilePath
                        });
                        uploadRunData = false;
                        if (File.Exists(runFilePath))
                        {
                            File.Delete(runFilePath);
                        }
                    }

                    // Upload scenario data
                    if (uploadScenarioData && !String.IsNullOrWhiteSpace(scenarioFilePath) && File.Exists(scenarioFilePath))
                    {
                        _cloudStorage.Upload(new S3UploadComment()
                        {
                            BucketName = _awsSettings.S3Bucket,
                            DestinationFilePath = scenarioFileNameWithPath,
                            SourceFilePath = scenarioFilePath
                        });
                        if (File.Exists(scenarioFilePath))
                        {
                            File.Delete(scenarioFilePath);
                        }
                    }
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0,
                        PipelineEventIDs.FINISHED_UPLOADING_INPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE, run.Id, scenarioId,
                        null, null, null, null));

                    _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                        PipelineEventIDs.FINISHED_UPLOADING_INPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE, run.Id, scenarioId, null));
                }
            }
            catch (System.Exception exception)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0,
                    PipelineEventIDs.FINISHED_UPLOADING_INPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE, run.Id, scenarioId, null, null,
                    exception.Message, exception));

                _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                    PipelineEventIDs.FINISHED_UPLOADING_INPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE, run.Id, scenarioId, exception.Message));

                throw;
            }

            finally
            {
                _pipelineAuditEventRepository.SaveChanges();
            }
        }

        /// <summary>
        /// Cleans up
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenarioId"></param>
        public void CleanUp(Run run, Guid scenarioId)
        {
            // No action
        }
    }
}
