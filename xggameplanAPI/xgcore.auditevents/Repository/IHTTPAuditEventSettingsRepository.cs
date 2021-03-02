using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Interface for HTTP audit event settings repository
    /// </summary>
    public interface IHTTPAuditEventSettingsRepository
    {
        void Insert(List<HTTPAuditEventSettings> items);

        List<HTTPAuditEventSettings> GetAll();

        void DeleteAll();

        void DeleteByID(int id);

        void Update(HTTPAuditEventSettings item);

        void SaveChanges();
    }
}
