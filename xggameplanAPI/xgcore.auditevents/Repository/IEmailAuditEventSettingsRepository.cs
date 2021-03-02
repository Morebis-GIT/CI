using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Interface for Emais audit event settings repository
    /// </summary>{
    public interface IEmailAuditEventSettingsRepository
    {
        void AddRange(IEnumerable<EmailAuditEventSettings> items);

        List<EmailAuditEventSettings> GetAll();

        void Truncate();

        void Delete(int eventTypeid);

        void Update(EmailAuditEventSettings item);

        void SaveChanges();
    }
}
