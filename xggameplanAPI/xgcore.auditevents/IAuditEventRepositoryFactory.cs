using System.Collections.Generic;
using xggameplan.common.Email;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Factory for providing IAuditEventRepository instances
    ///
    /// TODO: Remove this
    /// </summary>
    public interface IAuditEventRepositoryFactory
    {
        IAuditEventRepository GetCSVAuditEventRepository(string csvLogsFolder, List<AuditEventValueConverter> valueConverters, ICSVAuditEventSettingsRepository csvAuditEventSettingsRepository);

        IAuditEventRepository GetSQLAuditEventRepository(List<AuditEventValueConverter> valueConverters, ISQLAuditEventSettingsRepository sqlAuditEventSettingsRepository);

        IAuditEventRepository GetEmailAuditEventRepository(IEmailConnection emailConnection, List<AuditEventValueConverter> valueConverters, List<IAuditEventEmailCreator> emailCreators, IEmailAuditEventSettingsRepository emailAuditEventSettingsRepository);

        IAuditEventRepository GetMSTeamsAuditEventRepository(List<AuditEventValueConverter> valueConverters, List<IMSTeamsMessageCreator> messageCreators, IMSTeamsAuditEventSettingsRepository msTeamsAuditEventSettingsRepository);

        IAuditEventRepository GeteEventLogAuditEventRepository(List<AuditEventValueConverter> valueConverters);

        IAuditEventRepository GetFileAuditEventRepository(string folder, List<AuditEventValueConverter> valueConverters, IFileAuditEventSettingsRepository fileAuditEventSettingsRespository);

        IAuditEventRepository GetHTTPAuditEventRepository(List<AuditEventValueConverter> valueConverters, IHTTPAuditEventSettingsRepository httpAuditEventSettingsRepository);

        IAuditEventRepository GetConsoleAuditEventRepository();

        IAuditEventRepository GetDatadogAuditEventRepository();

        IAuditEventRepository GetAWSCloudWatchAuditEventRepository();
    }
}
