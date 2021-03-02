using System.Collections.Generic;
using System.Text;
using xggameplan.AuditEvents.ValueConverter;
using xggameplan.common.Email;

namespace xggameplan.AuditEvents.Sample
{
    /// <summary>
    /// Configuration for email events
    /// </summary>
    internal class EmailConfiguration : IAuditEventRepositoryCreator
    {
        private IAuditEventTypeRepository _auditEventTypeRepository;
        private IAuditEventValueTypeRepository _auditEventValueTypeRepository;
        private IEmailAuditEventSettingsRepository _emailAuditEventSettingsRepository;

        public EmailConfiguration(IAuditEventTypeRepository auditEventTypeRepository, IAuditEventValueTypeRepository auditEventValueTypeRepository, IEmailAuditEventSettingsRepository emailAuditEventSettingsRepository)
        {
            _auditEventTypeRepository = auditEventTypeRepository;
            _auditEventValueTypeRepository = auditEventValueTypeRepository;
            _emailAuditEventSettingsRepository = emailAuditEventSettingsRepository;
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
            return valueConverters;
        }

        private List<IAuditEventEmailCreator> GetEmailCreators()
        {
            // Define list of email creators
            List<IAuditEventEmailCreator> emailCreators = new List<IAuditEventEmailCreator>()
            {
                new BasicEmailCreator(_auditEventTypeRepository, _auditEventValueTypeRepository, _emailAuditEventSettingsRepository.GetAll(), GetValueConverters(), GetEmailHTMLStyle())
            };
            return emailCreators;
        }

        private IEmailConnection GetEmailConnection()
        {
            return new SendGridHttpEmailConnection(GetEmailConnectionSettingsSendGrid());
            //return new SendGridSmtpEmailConnection(GetEmailConnectionSettingsSmtp());
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

        /// <summary>
        /// Gets email connection settings for SendGrid emails
        /// </summary>
        /// <returns></returns>
        private EmailConnectionSettings GetEmailConnectionSettingsSendGrid()
        {
            // TODO: Set these settings
            EmailConnectionSettings emailConnectionSettings = new EmailConnectionSettings();
            emailConnectionSettings.Settings.Add("Type", "HTTPSendGrid");
            emailConnectionSettings.Settings.Add("APIKey", "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");
            return emailConnectionSettings;
        }

        /// <summary>
        /// Gets email connection settings for SMTP emails
        /// </summary>
        /// <returns></returns>
        private EmailConnectionSettings GetEmailConnectionSettingsSmtp()
        {
            // TODO: Set these settings
            EmailConnectionSettings emailConnectionSettings = new EmailConnectionSettings();
            emailConnectionSettings.Settings.Add("Type", "SMTP");
            emailConnectionSettings.Settings.Add("Server", "MyServer");
            emailConnectionSettings.Settings.Add("Port", "1001");
            emailConnectionSettings.Settings.Add("Username", "MyUsername");
            emailConnectionSettings.Settings.Add("Password", "MyPassword");
            return emailConnectionSettings;
        }
    }
}
