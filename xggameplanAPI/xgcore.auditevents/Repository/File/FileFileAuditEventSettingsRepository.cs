using System.Collections.Generic;
using System.Linq;
using xgcore.auditevents.Repository.File;
using xggameplan.AuditEvents;

namespace xggameplan.Repository.File
{
    public class FileFileAuditEventSettingsRepository : FileRepositoryBase, IFileAuditEventSettingsRepository
    {
        public FileFileAuditEventSettingsRepository(string folder) : base(folder, "file_audit_event_settings")
        {

        }

        public void Insert(List<FileAuditEventSettings> items)
        {
            InsertItems<FileAuditEventSettings>(_folder, _type, items, items.Select(i => i.EventTypeId.ToString()).ToList());
        }

        public List<FileAuditEventSettings> GetAll()
        {
            return GetAllByType<FileAuditEventSettings>(_folder, _type);
        }

        public void Update(FileAuditEventSettings item)
        {
            UpdateOrInsertItem<FileAuditEventSettings>(_folder, _type, item, item.EventTypeId.ToString());
        }

        public FileAuditEventSettings GetByID(int id)
        {
            return GetItemByID<FileAuditEventSettings>(_folder, _type, id.ToString());
        }

        public void DeleteByID(int id)
        {
            DeleteItem<FileAuditEventSettings>(_folder, _type, id.ToString());
        }

        public void DeleteAll()
        {
            DeleteAllItems<FileAuditEventSettings>(_folder, _type);
        }

        public void SaveChanges()
        {

        }
    }
}
