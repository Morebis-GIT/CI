using System.Collections.Generic;
using xgcore.auditevents.Repository.File;
using xggameplan.AuditEvents;

namespace xggameplan.Repository.File
{
    public class FileCSVAuditEventSettingsRepository : FileRepositoryBase, ICSVAuditEventSettingsRepository
    {
        public FileCSVAuditEventSettingsRepository(string folder) : base(folder, "csv_audit_event_settings")
        {
        }

        public void Insert(List<CSVAuditEventSettings> items)
        {
            InsertItems<CSVAuditEventSettings>(_folder, _type, items, items.ConvertAll(i => i.EventTypeId.ToString()));
        }

        public List<CSVAuditEventSettings> GetAll()
        {
            return GetAllByType<CSVAuditEventSettings>(_folder, _type);
        }

        public void Update(CSVAuditEventSettings item)
        {
            UpdateOrInsertItem<CSVAuditEventSettings>(_folder, _type, item, item.EventTypeId.ToString());
        }

        public CSVAuditEventSettings GetByID(int id)
        {
            return GetItemByID<CSVAuditEventSettings>(_folder, _type, id.ToString());
        }

        public void DeleteByID(int id)
        {
            DeleteItem(_folder, _type, id.ToString());
        }

        public void DeleteAll()
        {
            DeleteAllItems(_folder, _type);
        }

        public void SaveChanges()
        {
        }
    }
}
