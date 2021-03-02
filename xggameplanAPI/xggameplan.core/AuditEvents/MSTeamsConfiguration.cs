using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using xggameplan.AuditEvents.ValueConverter;
using xggameplan.common.MSTeams;

namespace xggameplan.AuditEvents
{
    public class MSTeamsConfiguration : IAuditEventRepositoryCreator
    {
        private readonly string _frontendUrl;
        private readonly IAuditEventTypeRepository _auditEventTypeRepository;
        private readonly IMSTeamsAuditEventSettingsRepository _msTeamsAuditEventSettingsRepository;
        private readonly IRepositoryFactory _repositoryFactory;

        public MSTeamsConfiguration(IAuditEventTypeRepository auditEventTypeRepository, IMSTeamsAuditEventSettingsRepository msTeamsAuditEventSettingsRepository,
                                    string frontendUrl, IRepositoryFactory repositoryFactory)
        {
            _frontendUrl = frontendUrl;
            _auditEventTypeRepository = auditEventTypeRepository;
            _msTeamsAuditEventSettingsRepository = msTeamsAuditEventSettingsRepository;
            _repositoryFactory = repositoryFactory;
        }

        public IAuditEventRepository GetAuditEventRepository()
        {
            return new MSTeamsAuditEventRepository("", _msTeamsAuditEventSettingsRepository.GetAll(), GetMessageCreators(), _auditEventTypeRepository);
        }

        private List<AuditEventValueConverter> GetValueConverters()
        {
            List<AuditEventValueConverter> valueConverters = new List<AuditEventValueConverter>();
            valueConverters.Add(new AuditEventValueConverter(new List<int>() { AuditEventValueTypes.Exception }, new ExceptionToHTMLConverter()));
            valueConverters.Add(new AuditEventValueConverter(new List<int>() { AuditEventValueTypes.GamePlanPipelineEventID }, PipelineEventIDToDescriptionConverter));
            valueConverters.Add(new AuditEventValueConverter(new List<int>() { AuditEventValueTypes.GamePlanAutoBookEventID }, AutoBookEventIDToDescriptionConverter));
            return valueConverters;
        }

        private List<IMSTeamsMessageCreator> GetMessageCreators()
        {
            // Create MS Teams REST API
            var msTeamsREST = new MSTeamsREST();

            var msTeamsValueConverters = GetValueConverters();

            // Set message creators
            List<IMSTeamsMessageCreator> messageCreators = new List<IMSTeamsMessageCreator>()
                {
                        new MSTeamsCardMessageCreator(_frontendUrl, _repositoryFactory, msTeamsREST, _msTeamsAuditEventSettingsRepository.GetAll(), msTeamsValueConverters),
                        new MSTeamsSimpleMessageCreator(_frontendUrl, _repositoryFactory, msTeamsREST, _msTeamsAuditEventSettingsRepository.GetAll(), msTeamsValueConverters)
                };        
            return messageCreators;
        }

        private MappingConverter<int, string> AutoBookEventIDToDescriptionConverter
        {
            get
            {
                Dictionary<int, string> list = new Dictionary<int, string>()
                {
                    { AutoBookEventIDs.AutoBookCreated, "AutoBook created" },
                    { AutoBookEventIDs.AutoBookDeleted, "AutoBook deleted" },
                    { AutoBookEventIDs.AutoBookProvisioned, "AutoBook provisioned" },
                    { AutoBookEventIDs.AutoBookRestarted, "AutoBook restarted" },
                };
                return new MappingConverter<int, string>(list);
            }
        }

        private MappingConverter<int, string> PipelineEventIDToDescriptionConverter
        {
            get
            {
                // This should be in the database...
                Dictionary<int, string> list = new Dictionary<int, string>()
                {
                    { PipelineEventIDs.FINISHED_DOWNLOADING_INPUT_ZIP_ARCHIVE_FROM_CLOUD_STORAGE, "Finished downloading input files from cloud" },
                    { PipelineEventIDs.FINISHED_DOWNLOADING_ZIP_ARCHIVE_FROM_CLOUD_STORAGE, "Finished downloading files from cloud" },
                    { PipelineEventIDs.FINISHED_EXPORTING_DATA_TO_DATABASE, "Finished exporting data to database" },
                    { PipelineEventIDs.FINISHED_GENERATING_INPUT_FILES, "Finished generating input files" },
                    { PipelineEventIDs.FINISHED_NOTIFYING_AUTOBOOK_API, "Finished notifying AutoBook to start processing" },
                    { PipelineEventIDs.FINISHED_NOTIFYING_GAMEPLAN_API_STATUS_IDLE, "Finished notifying Gameplan API of Idle status" },
                    { PipelineEventIDs.FINISHED_NOTIFYING_GAMEPLAN_API_TASK_DONE, "Finished notifying Gameplan API of run completed" },
                    { PipelineEventIDs.FINISHED_NOTIFYING_MULE_SOFT_API, "Finished notifying Mulesoft" },
                    { PipelineEventIDs.FINISHED_RUNNING_AUTOBOOK_CPP_OPTIMIZER, "Finished running Optimizer" },
                    { PipelineEventIDs.FINISHED_SMOOTHING_INPUT_FILES, "Finished Smoothing input files" },
                    { PipelineEventIDs.FINISHED_UNZIPPING_INPUT_FILES, "Finished unzipping input files" },
                    { PipelineEventIDs.FINISHED_UPLOADING_INPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE, "Finised uploading input files to cloud" },
                    { PipelineEventIDs.FINISHED_UPLOADING_OUTPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE, "Finished uploading output files to cloud" },
                    { PipelineEventIDs.FINISHED_ZIPPING_INPUT_FILES, "Finished zipping input files" },
                    { PipelineEventIDs.FINISHED_ZIPPING_OUTPUT_FILES, "Finished zipping output files" },
                    { PipelineEventIDs.STARTED_DOWNLOADING_INPUT_ZIP_ARCHIVE_FROM_CLOUD_STORAGE, "Downloading input files from cloud" },
                    { PipelineEventIDs.STARTED_DOWNLOADING_ZIP_ARCHIVE_FROM_CLOUD_STORAGE, "Downloading output files from cloud" },
                    { PipelineEventIDs.STARTED_EXPORTING_DATA_TO_DATABASE, "Exporting data to database" },
                    { PipelineEventIDs.STARTED_GENERATING_INPUT_FILES, "Generating input files" },
                    { PipelineEventIDs.STARTED_NOTIFYING_AUTOBOOK_API, "Notifying AutoBook API to start processing" },
                    { PipelineEventIDs.STARTED_NOTIFYING_GAMEPLAN_API_STATUS_IDLE, "Notifying Gameplan API of Idle status" },
                    { PipelineEventIDs.STARTED_NOTIFYING_GAMEPLAN_API_TASK_DONE, "Notifying Gameplan API of run completed" },
                    { PipelineEventIDs.STARTED_NOTIFYING_MULE_SOFT_API, "Notifying Mulesoft" },
                    { PipelineEventIDs.STARTED_RUNNING_AUTOBOOK_CPP_OPTIMIZER, "Running Optimizer" },
                    { PipelineEventIDs.STARTED_SMOOTHING_INPUT_FILES, "Smoothing input files" },
                    { PipelineEventIDs.STARTED_UNZIPPING_INPUT_FILES, "Unzipping input files" },
                    { PipelineEventIDs.STARTED_UPLOADING_INPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE, "Uploading input files to cloud" },
                    { PipelineEventIDs.STARTED_UPLOADING_OUTPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE, "Uploading output files to cloud" },
                    { PipelineEventIDs.STARTED_ZIPPING_INPUT_FILES, "Zipping input files" },
                    { PipelineEventIDs.STARTED_ZIPPING_OUTPUT_FILES, "Zipping output files" }
                };
                return new MappingConverter<int, string>(list);
            }
        }

    }
}
