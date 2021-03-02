using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Shared.System.Settings;
using Newtonsoft.Json;
using xggameplan.Repository.File;

namespace xggameplan.AuditEvents.Sample
{
    /// <summary>
    /// Runs sample to demonstrate the audit event mechanism. All data is stored locally in the folders passed to the class constructor.
    /// </summary>
    public class SampleRunner
    {
        private readonly string _configurationFolder;
        private readonly string _csvFolder;
        private readonly string _textFileFolder;
        private readonly string _queryResultsFolder;
        private readonly IAuditEventTypeRepository _auditEventTypeRepository;
        private readonly IAuditEventValueTypeRepository _auditEventValueTypeRepository;

        private const string _eventSource = "AuditEventsSample";
        private const string _userName = "Joe Bloggs";

        public SampleRunner(string dataFolder)
        {
            _configurationFolder = Path.Combine(dataFolder, "Configuration");
            _csvFolder = Path.Combine(dataFolder, "CSVEvents");
            _textFileFolder = Path.Combine(dataFolder, "TextFileEvents");
            _queryResultsFolder = Path.Combine(dataFolder, "QueryResults");

            // Create folders
            foreach (string folder in new string[] { _configurationFolder, _csvFolder, _textFileFolder, _queryResultsFolder })
            {
                _ = Directory.CreateDirectory(folder);
            }

            // Set repository for audit event types
            _auditEventTypeRepository = new FileAuditEventTypeRepository(System.IO.Path.Combine(_configurationFolder, "AuditEventTypes"));

            // Set repository for audit event value types
            _auditEventValueTypeRepository = new FileAuditEventValueTypeRepository(System.IO.Path.Combine(_configurationFolder, "AuditEventValueTypes"));
        }

        private void DeleteFiles(string folder)
        {
            foreach (string file in Directory.GetFiles(folder))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Creates test events. Events are handled as per the individual configuration for the individual event type. At a minimum then events are usually
        /// persisted (using CSVAuditEventsRepository, SQLAuditEventsRepository, FileAuditEventsRepository etc) so that they can be queried later.
        ///
        /// Events would normally be created in one of the following ways:
        /// - The REST API can generate events itself by calling MasterAuditEventRepository.Insert
        /// - The client can generate events by sending a POST:\auditevents to REST API
        /// </summary>
        /// <param name="auditEventRepository"></param>
        public void CreateEvents()
        {
            EventCreation eventCreation = new EventCreation();

            // Get repositories contains audit event settings
            Dictionary<Type, object> auditEventSettingsRepositories = GetAuditEventSettingsRepositories();

            // Get master audit event repository
            IAuditEventRepository auditEventRepository = GetMasterAuditEventRepository(_auditEventTypeRepository, _auditEventValueTypeRepository, auditEventSettingsRepositories, _csvFolder, _textFileFolder);

            // Create test events
            eventCreation.CreateEvents(auditEventRepository, _eventSource);
        }

        /// <summary>
        /// Queries previously created events using various queries
        ///
        /// Events would normally be queried in one of the following ways:
        /// - The REST API can query events itself by calling MasterAuditEventRepository.Get(AuditEventFilter)
        /// - The client can query events by sending a GET:\auditevents to REST API
        ///
        /// The plan is to improve the filtering mechanism later to support features such as pattern matching in strings and filtering values in
        /// ranges.
        /// </summary>
        /// <param name="auditEventRepository"></param>
        public void QueryEvents()
        {
            EventQuerying eventQuerying = new EventQuerying();

            // Get repositories contains audit event settings
            Dictionary<Type, object> auditEventSettingsRepositories = GetAuditEventSettingsRepositories();

            DeleteFiles(_queryResultsFolder);

            // Get master audit event repository
            IAuditEventRepository auditEventRepository = GetMasterAuditEventRepository(_auditEventTypeRepository, _auditEventValueTypeRepository, auditEventSettingsRepositories, _csvFolder, _textFileFolder);

            // Get events for high priority debug levels
            var eventsForDebugLevel = eventQuerying.GetEventsForDebugLevel(auditEventRepository, new List<int>() { DebugLevels.Exception, DebugLevels.Warning });
            SaveEvents(eventsForDebugLevel, "EventsForDebugLevel.json");

            // Get events by EventTypeId (E.g. All events related to programmes)
            var eventTypeIdsForProgrammes = new List<int>() { AuditEventTypes.AddProgramme, AuditEventTypes.UpdateProgramme, AuditEventTypes.DeleteProgramme, AuditEventTypes.ScheduleProgramme };
            var eventsForProgrammes = eventQuerying.GetEventsForEventTypeIds(auditEventRepository, eventTypeIdsForProgrammes);
            SaveEvents(eventsForProgrammes, "EventsForProgrammes.json");

            // Get events by source
            var eventsForSource = eventQuerying.GetEventsForSource(auditEventRepository, _eventSource);
            SaveEvents(eventsForSource, "EventsForSource.json");

            // Get events by user name
            var eventsForUserNames = eventQuerying.GetEventsForUserName(auditEventRepository, new List<string>() { _userName });
            SaveEvents(eventsForUserNames, "EventsForUserNames.json");

            // Get all events
            var eventsForAllTime = eventQuerying.GetEventsForAllTime(auditEventRepository);
            SaveEvents(eventsForAllTime, "EventsAll.json");
        }

        private void SaveEvents(List<AuditEvent> auditEvents, string filename)
        {
            string contentString = JsonConvert.SerializeObject(auditEvents, Formatting.Indented);
            File.WriteAllText(Path.Combine(_queryResultsFolder, filename), contentString);
        }

        /// <summary>
        /// Creates event types and values
        /// </summary>
        /// <param name="auditEventTypeRepository"></param>
        /// <param name="auditEventValueTypeRepository"></param>
        public void CreateEventTypesAndValues()
        {
            // Create audit event types
            CreateAuditEventTypes(_auditEventTypeRepository);

            // Create audit event value types
            CreateAuditEventValueTypes(_auditEventValueTypeRepository);
        }

        /// <summary>
        /// Configures audit event settings, how the events will be handled
        /// </summary>
        public void ConfigureSettings(string msTeamsURL, List<string> emailRecipientAddresses)
        {
            // Configure audit event settings
            Dictionary<Type, object> auditEventSettingsRepositories = GetAuditEventSettingsRepositories();
            ConfigureAuditEventSettings(_auditEventTypeRepository, _auditEventValueTypeRepository, auditEventSettingsRepositories, msTeamsURL, emailRecipientAddresses);
        }

        /// <summary>
        /// Creates list of audit event types
        /// </summary>
        /// <param name="auditEventTypeRepository"></param>
        /// <param name="auditEventValueTypeRepository"></param>
        private void CreateAuditEventTypes(IAuditEventTypeRepository auditEventTypeRepository)
        {
            List<AuditEventType> auditEventTypes = new List<AuditEventType>();
            auditEventTypes.Add(new AuditEventType(AuditEventTypes.Debug, "Debug"));
            auditEventTypes.Add(new AuditEventType(AuditEventTypes.TestValues, "Test values"));
            auditEventTypes.Add(new AuditEventType(AuditEventTypes.AddProgramme, "Add programme"));
            auditEventTypes.Add(new AuditEventType(AuditEventTypes.UpdateProgramme, "Update programme"));
            auditEventTypes.Add(new AuditEventType(AuditEventTypes.DeleteProgramme, "Delete programme"));
            auditEventTypes.Add(new AuditEventType(AuditEventTypes.ScheduleProgramme, "Schedule programme"));

            // Save, replace all existing
            auditEventTypeRepository.DeleteAll();
            auditEventTypeRepository.Insert(auditEventTypes);
        }

        private void CreateAuditEventValueTypes(IAuditEventValueTypeRepository auditEventValueTypeRepository)
        {
            List<AuditEventValueType> auditEventValueTypes = new List<AuditEventValueType>();
            auditEventValueTypes.Add(new AuditEventValueType(AuditEventValueTypes.Message, "Message", typeof(String), false));
            auditEventValueTypes.Add(new AuditEventValueType(AuditEventValueTypes.Exception, "Exception", typeof(ExceptionModel), false));
            auditEventValueTypes.Add(new AuditEventValueType(AuditEventValueTypes.TestValues, "Test values", typeof(TestValues), false));
            auditEventValueTypes.Add(new AuditEventValueType(AuditEventValueTypes.DebugLevel, "Debug level", typeof(Int16), false));
            auditEventValueTypes.Add(new AuditEventValueType(AuditEventValueTypes.Programme, "Programme", typeof(Programme), false));
            auditEventValueTypes.Add(new AuditEventValueType(AuditEventValueTypes.ChannelId, "Channel ID", typeof(Guid), false));
            auditEventValueTypes.Add(new AuditEventValueType(AuditEventValueTypes.UserName, "UserName", typeof(String), false));

            // Save, replace all existing
            auditEventValueTypeRepository.DeleteAll();
            auditEventValueTypeRepository.Insert(auditEventValueTypes);
        }

        /// <summary>
        /// Returns audit event settings repositories
        /// </summary>
        /// <returns></returns>
        private Dictionary<Type, object> GetAuditEventSettingsRepositories()
        {
            Dictionary<Type, object> auditEventSettingsRepositories = new Dictionary<Type, object>()
            {
                { typeof(ICSVAuditEventSettingsRepository), new FileCSVAuditEventSettingsRepository(System.IO.Path.Combine(_configurationFolder, "CSVAuditEventSettings")) },
                { typeof(IEmailAuditEventSettingsRepository), new FileEmailAuditEventSettingsRepository(System.IO.Path.Combine(_configurationFolder, "EmailAuditEventSettings")) },
                { typeof(IMSTeamsAuditEventSettingsRepository), new FileMSTeamsAuditEventSettingsRepository(System.IO.Path.Combine(_configurationFolder, "MSTeamsAuditEventSettings")) },
                { typeof(IHTTPAuditEventSettingsRepository), new FileHTTPAuditEventSettingsRespository(System.IO.Path.Combine(_configurationFolder, "HTTPAuditEventSettings")) },
                { typeof(ISQLAuditEventSettingsRepository), new FileSQLAuditEventSettingsRepository(System.IO.Path.Combine(_configurationFolder, "SQLAuditEventSettings")) },
                { typeof(IFileAuditEventSettingsRepository), new FileFileAuditEventSettingsRepository(System.IO.Path.Combine(_configurationFolder, "FileAuditEventSettings")) },
                { typeof(ITextFileAuditEventSettingsRepository), new FileTextFileAuditEventSettingsRepository(System.IO.Path.Combine(_configurationFolder, "TextFileAuditEventSettings")) }
            };
            return auditEventSettingsRepositories;
        }

        /// <summary>
        /// Configure audit event settings. This is a one time process.
        /// </summary>
        private void ConfigureAuditEventSettings(IAuditEventTypeRepository auditEventTypeRepository, IAuditEventValueTypeRepository auditEventValueTypeRepository,
                                                 Dictionary<Type, object> auditEventSettingsRepositories,
                                                 string msTeamsUrl, List<string> emailRecipientAddresses)
        {
            ConfigureAuditEvent(AuditEventTypes.Debug, auditEventSettingsRepositories, true, true, true, false, false, true, msTeamsUrl, emailRecipientAddresses);
            ConfigureAuditEvent(AuditEventTypes.TestValues, auditEventSettingsRepositories, true, true, true, false, false, true, msTeamsUrl, emailRecipientAddresses);
            ConfigureAuditEvent(AuditEventTypes.AddProgramme, auditEventSettingsRepositories, true, true, true, false, false, true, msTeamsUrl, emailRecipientAddresses);
            ConfigureAuditEvent(AuditEventTypes.UpdateProgramme, auditEventSettingsRepositories, true, true, true, false, false, true, msTeamsUrl, emailRecipientAddresses);
            ConfigureAuditEvent(AuditEventTypes.DeleteProgramme, auditEventSettingsRepositories, true, true, true, false, false, true, msTeamsUrl, emailRecipientAddresses);
            ConfigureAuditEvent(AuditEventTypes.ScheduleProgramme, auditEventSettingsRepositories, true, true, true, false, false, true, msTeamsUrl, emailRecipientAddresses);
        }

        /// <summary>
        /// Configures an audit event
        /// </summary>
        /// <param name="eventTypeId"></param>
        /// <param name="auditEventSettingsRepositories"></param>
        private void ConfigureAuditEvent(int eventTypeId, Dictionary<Type, object> auditEventSettingsRepositories,
                                        bool csvEvent, bool emailEvent, bool msTeamsEvent, bool sqlEvent, bool httpEvent, bool textFileEvent,
                                        string msTeamsUrl, List<string> emailRecipientAddresses)
        {
            // Get audit event settings repositories
            IMSTeamsAuditEventSettingsRepository msTeamsAuditEventSettingsRepository = auditEventSettingsRepositories.ContainsKey(typeof(IMSTeamsAuditEventSettingsRepository)) ? (IMSTeamsAuditEventSettingsRepository)auditEventSettingsRepositories[typeof(IMSTeamsAuditEventSettingsRepository)] : null;
            IEmailAuditEventSettingsRepository emailAuditEventSettingsRepository = auditEventSettingsRepositories.ContainsKey(typeof(IEmailAuditEventSettingsRepository)) ? (IEmailAuditEventSettingsRepository)auditEventSettingsRepositories[typeof(IEmailAuditEventSettingsRepository)] : null;
            ICSVAuditEventSettingsRepository csvAuditEventSettingsRepository = auditEventSettingsRepositories.ContainsKey(typeof(ICSVAuditEventSettingsRepository)) ? (ICSVAuditEventSettingsRepository)auditEventSettingsRepositories[typeof(ICSVAuditEventSettingsRepository)] : null;
            ISQLAuditEventSettingsRepository sqlAuditEventSettingsRepository = auditEventSettingsRepositories.ContainsKey(typeof(ISQLAuditEventSettingsRepository)) ? (ISQLAuditEventSettingsRepository)auditEventSettingsRepositories[typeof(ISQLAuditEventSettingsRepository)] : null;
            IHTTPAuditEventSettingsRepository httpAuditEventSettingsRepository = auditEventSettingsRepositories.ContainsKey(typeof(IHTTPAuditEventSettingsRepository)) ? (IHTTPAuditEventSettingsRepository)auditEventSettingsRepositories[typeof(IHTTPAuditEventSettingsRepository)] : null;
            ITextFileAuditEventSettingsRepository textFileAuditEventSettingsRepository = auditEventSettingsRepositories.ContainsKey(typeof(ITextFileAuditEventSettingsRepository)) ? (ITextFileAuditEventSettingsRepository)auditEventSettingsRepositories[typeof(ITextFileAuditEventSettingsRepository)] : null;

            // Configure CSV
            if (csvAuditEventSettingsRepository != null)
            {
                CSVAuditEventSettings csvAuditEventSettings = new CSVAuditEventSettings()
                {
                    EventTypeId = eventTypeId,
                    Enabled = csvEvent
                };
                csvAuditEventSettingsRepository.Insert(new List<CSVAuditEventSettings>() { csvAuditEventSettings });
            }

            // Configure email
            if (emailAuditEventSettingsRepository != null)
            {
                EmailAuditEventSettings emailAuditEventSettings = new EmailAuditEventSettings()
                {
                    EventTypeId = eventTypeId,
                    EmailCreatorId = "Default",
                    NotificationSettings = new EmailNotificationSettings()
                    {
                        Enabled = emailEvent,
                        SenderAddress = "nextgen@imaginecommunications.com",
                        RecipientAddresses = emailRecipientAddresses
                    }
                };
                emailAuditEventSettingsRepository.AddRange(new List<EmailAuditEventSettings>() { emailAuditEventSettings });
            }

            // Configure MS Teams notification
            if (msTeamsAuditEventSettingsRepository != null)
            {
                MSTeamsAuditEventSettings msTeamsAuditEventSettings = new MSTeamsAuditEventSettings()
                {
                    EventTypeId = eventTypeId,
                    MessageCreatorId = "Basic",
                    PostMessageSettings = new MSTeamsPostMessageSettings()
                    {
                        Enabled = msTeamsEvent,
                        Url = msTeamsUrl
                    }
                };
                msTeamsAuditEventSettingsRepository.Insert(new List<MSTeamsAuditEventSettings>() { msTeamsAuditEventSettings });
            }

            // Configure SQL
            if (sqlAuditEventSettingsRepository != null)
            {
                SQLAuditEventSettings sqlAuditEventSettings = new SQLAuditEventSettings()
                {
                    EventTypeId = eventTypeId,
                    Enabled = sqlEvent,
                    ConnectionString = ""    // TODO: Set this
                };
                sqlAuditEventSettingsRepository.Insert(new List<SQLAuditEventSettings>() { sqlAuditEventSettings });
            }

            // Configure HTTP
            if (httpAuditEventSettingsRepository != null)
            {
                HTTPAuditEventSettings httpAuditEventSettings = new HTTPAuditEventSettings()
                {
                    EventTypeId = eventTypeId,
                    Enabled = httpEvent,
                    RequestSettings = new HTTPRequestSettings()
                    {
                        Method = "POST",
                        URL = @"http:\\myserver\event",
                        ContentTemplate = "{event_json}",
                        Headers = new Dictionary<string, string>()
                        {
                        }
                    },
                    ResponseSettings = new HTTPResponseSettings()
                    {
                        SuccessStatusCodes = new List<System.Net.HttpStatusCode>() { System.Net.HttpStatusCode.OK }
                    }
                };
                httpAuditEventSettingsRepository.Insert(new List<HTTPAuditEventSettings>() { httpAuditEventSettings });
            }

            // Configure text file
            if (textFileAuditEventSettingsRepository != null)
            {
                TextFileAuditEventSettings textFileAuditEventSettings = new TextFileAuditEventSettings()
                {
                    EventTypeId = eventTypeId,
                    Enabled = textFileEvent,
                    FormatterId = "Basic"
                };
                textFileAuditEventSettingsRepository.Insert(new List<TextFileAuditEventSettings>() { textFileAuditEventSettings });
            }
        }

        /// <summary>
        /// Returns master audit event repository
        /// </summary>
        /// <returns></returns>
        private IAuditEventRepository GetMasterAuditEventRepository(IAuditEventTypeRepository auditEventTypeRepository, IAuditEventValueTypeRepository auditEventValueTypeRepository,
                                                                Dictionary<Type, object> auditEventSettingsRepositories,
                                                                string csvFolder, string textFileFolder)
        {
            // Set child repositories
            // To disable a repository then comment it out
            List<IAuditEventRepositoryCreator> auditEventRepositoryCreators = new List<IAuditEventRepositoryCreator>();
            auditEventRepositoryCreators.Add(new CSVConfiguration(auditEventTypeRepository, auditEventValueTypeRepository, (ICSVAuditEventSettingsRepository)auditEventSettingsRepositories[typeof(ICSVAuditEventSettingsRepository)], csvFolder));
            auditEventRepositoryCreators.Add(new EmailConfiguration(auditEventTypeRepository, auditEventValueTypeRepository, (IEmailAuditEventSettingsRepository)auditEventSettingsRepositories[typeof(IEmailAuditEventSettingsRepository)]));
            auditEventRepositoryCreators.Add(new MSTeamsConfiguration(auditEventTypeRepository, auditEventValueTypeRepository, (IMSTeamsAuditEventSettingsRepository)auditEventSettingsRepositories[typeof(IMSTeamsAuditEventSettingsRepository)]));
            //auditEventRepositoryCreators.Add(new HTTPConfiguration(auditEventTypeRepository, auditEventValueTypeRepository, (IHTTPAuditEventSettingsRepository)auditEventSettingsRepositories[typeof(IHTTPAuditEventSettingsRepository)]));
            //auditEventRepositoryCreators.Add(new SQLConfiguration(auditEventTypeRepository, auditEventValueTypeRepository, (ISQLAuditEventSettingsRepository)auditEventSettingsRepositories[typeof(ISQLAuditEventSettingsRepository)]));
            auditEventRepositoryCreators.Add(new TextFileConfiguration(auditEventTypeRepository, auditEventValueTypeRepository, (ITextFileAuditEventSettingsRepository)auditEventSettingsRepositories[typeof(ITextFileAuditEventSettingsRepository)], textFileFolder));

            // Create child repositories
            List<IAuditEventRepository> auditEventRepositories = new List<IAuditEventRepository>();
            auditEventRepositoryCreators.ForEach(creator => auditEventRepositories.Add(creator.GetAuditEventRepository()));

            // Return master repository
            return new MasterAuditEventRepository(auditEventRepositories);
        }
    }
}
