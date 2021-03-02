using System.Collections.Generic;

namespace xggameplan.AuditEvents.Sample
{
    /// <summary>
    /// Configuration for text file events
    /// </summary>
    public class TextFileConfiguration : IAuditEventRepositoryCreator
    {
        private string _folder;
        private IAuditEventTypeRepository _auditEventTypeRepository;
        private IAuditEventValueTypeRepository _auditEventValueTypeRepository;
        private ITextFileAuditEventSettingsRepository _textFileAuditEventSettingsRepository;

        public TextFileConfiguration(IAuditEventTypeRepository auditEventTypeRepository, IAuditEventValueTypeRepository auditEventValueTypeRepository,
                                    ITextFileAuditEventSettingsRepository textFileAuditEventSettingsRepository, string folder)
        {
            _folder = folder;
            _auditEventTypeRepository = auditEventTypeRepository;
            _auditEventValueTypeRepository = auditEventValueTypeRepository;
            _textFileAuditEventSettingsRepository = textFileAuditEventSettingsRepository;
        }

        public IAuditEventRepository GetAuditEventRepository()
        {
            return new TextFileAuditEventRepository(_auditEventTypeRepository, GetFormatters(), _folder);
        }

        private List<ITextFileAuditEventFormatter> GetFormatters()
        {
            List<ITextFileAuditEventFormatter> formatters = new List<ITextFileAuditEventFormatter>()
            {
                new BasicTextFileAuditEventFormatter(_auditEventTypeRepository, _auditEventValueTypeRepository, _textFileAuditEventSettingsRepository.GetAll(), "\t")
            };
            return formatters;
        }
    }
}
