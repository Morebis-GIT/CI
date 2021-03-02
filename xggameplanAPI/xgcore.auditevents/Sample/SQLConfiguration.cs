namespace xggameplan.AuditEvents.Sample
{
    /// <summary>
    /// Configuration for SQL events
    /// </summary>
    internal class SQLConfiguration : IAuditEventRepositoryCreator
    {
        private IAuditEventTypeRepository _auditEventTypeRepository;
        private IAuditEventValueTypeRepository _auditEventValueTypeRepository;
        private ISQLAuditEventSettingsRepository _sqlAuditEventSettingsRepository;

        public SQLConfiguration(IAuditEventTypeRepository auditEventTypeRepository, IAuditEventValueTypeRepository auditEventValueTypeRepository, ISQLAuditEventSettingsRepository sqlAuditEventSettingsRepository)
        {
            _auditEventTypeRepository = auditEventTypeRepository;
            _auditEventValueTypeRepository = auditEventValueTypeRepository;
            _sqlAuditEventSettingsRepository = sqlAuditEventSettingsRepository;
        }

        public IAuditEventRepository GetAuditEventRepository()
        {
            return new SQLAuditEventRepository(_sqlAuditEventSettingsRepository.GetAll());
        }
    }
}
