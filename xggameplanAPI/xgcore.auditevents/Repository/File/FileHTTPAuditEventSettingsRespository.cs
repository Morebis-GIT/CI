using System.Collections.Generic;
using xgcore.auditevents.Repository.File;
using xggameplan.AuditEvents;

namespace xggameplan.Repository.File
{
    /// <summary>
    /// Repository for HTTPAuditEventSettings instances (File)
    /// </summary>
    public class FileHTTPAuditEventSettingsRespository : FileRepositoryBase, IHTTPAuditEventSettingsRepository
    {
        public FileHTTPAuditEventSettingsRespository(string folder) : base(folder, "http_audit_event_settings")
        {
        }

        public void Insert(List<HTTPAuditEventSettings> items)
        {
            InsertItems<HTTPAuditEventSettings>(_folder, _type, items, items.ConvertAll(i => i.EventTypeId.ToString()));
        }

        public List<HTTPAuditEventSettings> GetAll()
        {
            return GetAllByType<HTTPAuditEventSettings>(_folder, _type);
        }

        public void Update(HTTPAuditEventSettings item)
        {
            UpdateOrInsertItem<HTTPAuditEventSettings>(_folder, _type, item, item.EventTypeId.ToString());
        }

        public HTTPAuditEventSettings GetByID(int id)
        {
            return GetItemByID<HTTPAuditEventSettings>(_folder, _type, id.ToString());
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
