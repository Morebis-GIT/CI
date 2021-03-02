using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Interface for CSV audit event settings repository
    /// </summary>
    public interface ICSVAuditEventSettingsRepository
    {
        void Insert(List<CSVAuditEventSettings> items);

        List<CSVAuditEventSettings> GetAll();

        void DeleteAll();

        void DeleteByID(int id);

        void Update(CSVAuditEventSettings item);


        void SaveChanges();
    }
}
