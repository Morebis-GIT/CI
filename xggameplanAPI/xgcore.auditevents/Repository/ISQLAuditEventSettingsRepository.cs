using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Interface for SQL audit event settings repository
    /// </summary>
    public interface ISQLAuditEventSettingsRepository
    {
        void Insert(List<SQLAuditEventSettings> items);

        List<SQLAuditEventSettings> GetAll();

        void DeleteAll();

        void DeleteByID(int id);

        void Update(SQLAuditEventSettings item);


        void SaveChanges();
    }
}
