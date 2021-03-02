using System.Collections.Generic;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using xggameplan.AuditEvents.ValueConverter;
using xggameplan.common.Email;

namespace xggameplan.AuditEvents
{
    public class EmailConfiguration : IAuditEventRepositoryCreator
    {
        private string _environmentId;
        private string _frontendUrl;
        private IRepositoryFactory _repositoryFactory;
        private IEmailAuditEventSettingsRepository _emailAuditEventSettingsRepository;
        private IEmailConnection _emailConnection;

        public EmailConfiguration(IEmailAuditEventSettingsRepository emailAuditEventSettingsRepository,
                                    string environmentId, string frontendUrl, IRepositoryFactory repositoryFactory, IEmailConnection emailConnection)
        {
            _environmentId = environmentId;
            _frontendUrl = frontendUrl;
            _emailAuditEventSettingsRepository = emailAuditEventSettingsRepository;
            _repositoryFactory = repositoryFactory;
            _emailConnection = emailConnection;
        }

        public IAuditEventRepository GetAuditEventRepository()
        {
            return new EmailAuditEventRepository(GetEmailConnection(), GetEmailCreators());
        }

        private List<AuditEventValueConverter> GetValueConverters()
        {
            // Set value converters
            List<AuditEventValueConverter> valueConverters = new List<AuditEventValueConverter>();
            valueConverters.Add(new AuditEventValueConverter(new List<int>() { AuditEventValueTypes.Exception }, new ExceptionToHTMLConverter()));
            valueConverters.Add(new AuditEventValueConverter(new List<int>() { AuditEventValueTypes.GamePlanPipelineEventID }, PipelineEventIDToDescriptionConverter));
            valueConverters.Add(new AuditEventValueConverter(new List<int>() { AuditEventValueTypes.GamePlanAutoBookEventID }, AutoBookEventIDToDescriptionConverter));
            return valueConverters;
        }

        private List<IAuditEventEmailCreator> GetEmailCreators()
        {
            // Set email creators
            var emailValueConverters = GetValueConverters();
            List<IAuditEventEmailCreator> emailCreators = new List<IAuditEventEmailCreator>();
            emailCreators.Add(new DefaultAuditEventEmailCreator(_environmentId, _emailAuditEventSettingsRepository.GetAll(), emailValueConverters));
            emailCreators.Add(new RunAuditEventEmailCreator(_environmentId, _frontendUrl, _repositoryFactory, _emailAuditEventSettingsRepository.GetAll(), emailValueConverters));
            return emailCreators;
        }

        private IEmailConnection GetEmailConnection()
        {
            return _emailConnection;            
        }

        private static string GetEmailHTMLStyle()
        {
            StringBuilder style = new StringBuilder("<style>" +
                        "table { border: 1px solid; } " +
                        "th { border: 1px solid; } " +
                        "td { border: 1px solid; } " +
                        //"a, u { text-decoration: none; } " +
                        "</style>");
            return style.ToString();
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
