using System.Collections.Generic;
using xggameplan.common.Email;

namespace xggameplan.AuditEvents
{
    public class AuditEventRepositoryFactory : IAuditEventRepositoryFactory
    {
        private readonly string _environmentId;
        private readonly IAuditEventTypeRepository _auditEventTypeRepository;
        private readonly IAuditEventValueTypeRepository _auditEventValueTypeRepository;

        public AuditEventRepositoryFactory(string environmentId, IAuditEventTypeRepository auditEventTypeRepository, IAuditEventValueTypeRepository auditEventValueTypeRepository)
        {
            _environmentId = environmentId;
            _auditEventTypeRepository = auditEventTypeRepository;
            _auditEventValueTypeRepository = auditEventValueTypeRepository;
        }

        public IAuditEventRepository GetCSVAuditEventRepository(string csvLogsFolder, List<AuditEventValueConverter> valueConverters, ICSVAuditEventSettingsRepository csvAuditEventSettingsRepository)
        {
            return new CSVAuditEventRepository(csvLogsFolder, valueConverters, csvAuditEventSettingsRepository == null ? null : csvAuditEventSettingsRepository.GetAll(), _auditEventValueTypeRepository);
        }

        public IAuditEventRepository GetSQLAuditEventRepository(List<AuditEventValueConverter> valueConverters, ISQLAuditEventSettingsRepository sqlAuditEventSettingsRepository)
        {
            return new SQLAuditEventRepository(sqlAuditEventSettingsRepository.GetAll());
        }

        public IAuditEventRepository GetEmailAuditEventRepository(IEmailConnection emailConnection, List<AuditEventValueConverter> valueConverters, List<IAuditEventEmailCreator> emailCreators, IEmailAuditEventSettingsRepository emailAuditEventSettingsRepository)
        {
            return new EmailAuditEventRepository(emailConnection, emailCreators);
        }

        public IAuditEventRepository GetMSTeamsAuditEventRepository(List<AuditEventValueConverter> valueConverters, List<IMSTeamsMessageCreator> messageCreators, IMSTeamsAuditEventSettingsRepository msTeamsAuditEventSettingsRepository)
        {
            return new MSTeamsAuditEventRepository(_environmentId, msTeamsAuditEventSettingsRepository.GetAll(), messageCreators, _auditEventTypeRepository);
        }

        public IAuditEventRepository GeteEventLogAuditEventRepository(List<AuditEventValueConverter> valueConverters)
        {
            return null;
        }

        public IAuditEventRepository GetFileAuditEventRepository(string folder, List<AuditEventValueConverter> valueConverters, IFileAuditEventSettingsRepository fileAuditEventSettingsRepository)
        {
            return new FileAuditEventRepository(folder, fileAuditEventSettingsRepository.GetAll());
        }

        public IAuditEventRepository GetHTTPAuditEventRepository(List<AuditEventValueConverter> valueConverters, IHTTPAuditEventSettingsRepository httpAuditEventSettingsRepository)
        {
            return new HTTPAuditEventRepository(httpAuditEventSettingsRepository.GetAll());
        }

        public IAuditEventRepository GetConsoleAuditEventRepository()
        {
            return new ConsoleAuditEventRepository(_auditEventTypeRepository);
        }

        public IAuditEventRepository GetDatadogAuditEventRepository()
        {
            return new DatadogAuditEventRepository();
        }

        public IAuditEventRepository GetAWSCloudWatchAuditEventRepository()
        {
            return new AWSCloudWatchAuditEventRepository();
        }
    }
}
