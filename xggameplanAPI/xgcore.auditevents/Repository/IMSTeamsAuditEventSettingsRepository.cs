using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Interface for MS Teams audit event settings repository
    /// </summary>
    public interface IMSTeamsAuditEventSettingsRepository
    {
        void Insert(List<MSTeamsAuditEventSettings> items);

        List<MSTeamsAuditEventSettings> GetAll();

        void DeleteAll();

        void DeleteByID(int id);

        void Update(MSTeamsAuditEventSettings item);


        void SaveChanges();
    }
}
