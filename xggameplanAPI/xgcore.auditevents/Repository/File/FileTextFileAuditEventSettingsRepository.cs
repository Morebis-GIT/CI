using System.Collections.Generic;
using System.Linq;
using xgcore.auditevents.Repository.File;
using xggameplan.AuditEvents;

namespace xggameplan.Repository.File
{
    public class FileTextFileAuditEventSettingsRepository : FileRepositoryBase, ITextFileAuditEventSettingsRepository
    {
        public FileTextFileAuditEventSettingsRepository(string folder) : base(folder, "text_file_audit_event_settings")
        {

        }

        public void Insert(List<TextFileAuditEventSettings> items)
        {
            InsertItems<TextFileAuditEventSettings>(_folder, _type, items, items.Select(i => i.EventTypeId.ToString()).ToList());
        }

        public List<TextFileAuditEventSettings> GetAll()
        {
            return GetAllByType<TextFileAuditEventSettings>(_folder, _type);
        }

        public void Update(TextFileAuditEventSettings item)
        {
            UpdateOrInsertItem<TextFileAuditEventSettings>(_folder, _type, item, item.EventTypeId.ToString());
        }

        public TextFileAuditEventSettings GetByID(int id)
        {
            return GetItemByID<TextFileAuditEventSettings>(_folder, _type, id.ToString());
        }

        public void DeleteByID(int id)
        {
            DeleteItem<TextFileAuditEventSettings>(_folder, _type, id.ToString());
        }

        public void DeleteAll()
        {
            DeleteAllItems<TextFileAuditEventSettings>(_folder, _type);
        }

        public void SaveChanges()
        {

        }
    }
}
