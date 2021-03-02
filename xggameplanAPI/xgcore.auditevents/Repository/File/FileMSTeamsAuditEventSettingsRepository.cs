using System.Collections.Generic;
using xgcore.auditevents.Repository.File;
using xggameplan.AuditEvents;

namespace xggameplan.Repository.File
{
    /// <summary>
    /// Repository for MSTeamsAuditEventSettings instances (File)
    /// </summary>
    public class FileMSTeamsAuditEventSettingsRepository : FileRepositoryBase, IMSTeamsAuditEventSettingsRepository
    {
        public FileMSTeamsAuditEventSettingsRepository(string folder) : base(folder, "msteams_audit_event_settings")
        {
        }

        public void Insert(List<MSTeamsAuditEventSettings> items)
        {
            InsertItems<MSTeamsAuditEventSettings>(_folder, _type, items, items.ConvertAll(i => i.EventTypeId.ToString()));
        }

        public List<MSTeamsAuditEventSettings> GetAll()
        {
            return GetAllByType<MSTeamsAuditEventSettings>(_folder, _type);
        }

        public void Update(MSTeamsAuditEventSettings item)
        {
            UpdateOrInsertItem<MSTeamsAuditEventSettings>(_folder, _type, item, item.EventTypeId.ToString());
        }

        public MSTeamsAuditEventSettings GetByID(int id)
        {
            return GetItemByID<MSTeamsAuditEventSettings>(_folder, _type, id.ToString());
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
