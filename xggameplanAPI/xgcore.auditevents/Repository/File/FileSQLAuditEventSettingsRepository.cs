using System.Collections.Generic;
using System.Linq;
using xgcore.auditevents.Repository.File;
using xggameplan.AuditEvents;

namespace xggameplan.Repository.File
{
    public class FileSQLAuditEventSettingsRepository : FileRepositoryBase, ISQLAuditEventSettingsRepository
    {
        public FileSQLAuditEventSettingsRepository(string folder) : base(folder, "sql_audit_event_settings")
        {

        }

        public void Insert(List<SQLAuditEventSettings> items)
        {
            InsertItems<SQLAuditEventSettings>(_folder, _type, items, items.Select(i => i.EventTypeId.ToString()).ToList());
        }

        public List<SQLAuditEventSettings> GetAll()
        {
            return GetAllByType<SQLAuditEventSettings>(_folder, _type);
        }

        public void Update(SQLAuditEventSettings item)
        {
            UpdateOrInsertItem<SQLAuditEventSettings>(_folder, _type, item, item.EventTypeId.ToString());
        }

        public SQLAuditEventSettings GetByID(int id)
        {
            return GetItemByID<SQLAuditEventSettings>(_folder, _type, id.ToString());
        }

        public void DeleteByID(int id)
        {
            DeleteItem<SQLAuditEventSettings>(_folder, _type, id.ToString());
        }

        public void DeleteAll()
        {
            DeleteAllItems<SQLAuditEventSettings>(_folder, _type);
        }

        public void SaveChanges()
        {

        }
    }
}
