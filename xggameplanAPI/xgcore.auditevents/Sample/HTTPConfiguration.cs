namespace xggameplan.AuditEvents.Sample
{

    /// <summary>
    /// Configuration for HTTP events
    /// </summary>
    internal class HTTPConfiguration : IAuditEventRepositoryCreator
    {
        private IAuditEventTypeRepository _auditEventTypeRepository;
        private IAuditEventValueTypeRepository _auditEventValueTypeRepository;
        private IHTTPAuditEventSettingsRepository _httpAuditEventSettingsRepository;

        public HTTPConfiguration(IAuditEventTypeRepository auditEventTypeRepository, IAuditEventValueTypeRepository auditEventValueTypeRepository, IHTTPAuditEventSettingsRepository httpAuditEventSettingsRepository)
        {
            _auditEventTypeRepository = auditEventTypeRepository;
            _auditEventValueTypeRepository = auditEventValueTypeRepository;
            _httpAuditEventSettingsRepository = httpAuditEventSettingsRepository;
        }

        public IAuditEventRepository GetAuditEventRepository()
        {
            return new HTTPAuditEventRepository(_httpAuditEventSettingsRepository.GetAll());
        }
    }
}
