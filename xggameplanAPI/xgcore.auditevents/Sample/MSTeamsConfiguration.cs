using System.Collections.Generic;
using xggameplan.AuditEvents.ValueConverter;
using xggameplan.common.MSTeams;

namespace xggameplan.AuditEvents.Sample
{
    /// <summary>
    /// Configuration for MS Teams events
    /// </summary>
    internal class MSTeamsConfiguration : IAuditEventRepositoryCreator
    {
        private IAuditEventTypeRepository _auditEventTypeRepository;
        private IAuditEventValueTypeRepository _auditEventValueTypeRepository;
        private IMSTeamsAuditEventSettingsRepository _msTeamsAuditEventSettingsRepository;

        public MSTeamsConfiguration(IAuditEventTypeRepository auditEventTypeRepository, IAuditEventValueTypeRepository auditEventValueTypeRepository, IMSTeamsAuditEventSettingsRepository msTeamsAuditEventSettingsRepository)
        {
            _auditEventTypeRepository = auditEventTypeRepository;
            _auditEventValueTypeRepository = auditEventValueTypeRepository;
            _msTeamsAuditEventSettingsRepository = msTeamsAuditEventSettingsRepository;
        }

        public IAuditEventRepository GetAuditEventRepository()
        {
            return new MSTeamsAuditEventRepository("", _msTeamsAuditEventSettingsRepository.GetAll(), GetMessageCreators(), _auditEventTypeRepository);
        }

        private List<AuditEventValueConverter> GetValueConverters()
        {
            // Set value converters
            List<AuditEventValueConverter> valueConverters = new List<AuditEventValueConverter>();
            valueConverters.Add(new AuditEventValueConverter(new List<int>() { AuditEventValueTypes.Exception }, new ExceptionToHTMLConverter()));
            return valueConverters;
        }

        private List<IMSTeamsMessageCreator> GetMessageCreators()
        {
            // Create MS Teams REST API
            var msTeamsREST = new MSTeamsREST();

            List<IMSTeamsMessageCreator> messageCreators = new List<IMSTeamsMessageCreator>()
            {
                new BasicMSTeamsMessageCreator(msTeamsREST, _msTeamsAuditEventSettingsRepository.GetAll(), GetValueConverters(), _auditEventTypeRepository, _auditEventValueTypeRepository)
            };
            return messageCreators;
        }

    }
}
