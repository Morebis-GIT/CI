using System.Collections.Generic;
using System.Linq;
using xgcore.auditevents.Repository.File;
using xggameplan.AuditEvents;

namespace xggameplan.Repository.File
{
    /// <summary>
    /// Repository for EmailAuditEventSettings instances (File)
    /// </summary>
    public class FileEmailAuditEventSettingsRepository : FileRepositoryBase, IEmailAuditEventSettingsRepository
    {
        public FileEmailAuditEventSettingsRepository(string folder)
            : base(folder, "email_audit_event_settings")
        {

        }

        public void AddRange(IEnumerable<EmailAuditEventSettings> items)
        {
            InsertItems<EmailAuditEventSettings>(_folder, _type, items.ToList(), items.Select(i => i.EventTypeId.ToString()).ToList());
        }

        public List<EmailAuditEventSettings> GetAll()
        {
            return GetAllByType<EmailAuditEventSettings>(_folder, _type);
        }

        public void Update(EmailAuditEventSettings item)
        {
            UpdateOrInsertItem<EmailAuditEventSettings>(_folder, _type, item, item.EventTypeId.ToString());
        }

        public EmailAuditEventSettings GetByID(int id)
        {
            return GetItemByID<EmailAuditEventSettings>(_folder, _type, id.ToString());
        }

        public void Delete(int eventTypeid)
        {
            DeleteItem<EmailAuditEventSettings>(_folder, _type, eventTypeid.ToString());
        }

        public void Truncate()
        {
            DeleteAllItems<EmailAuditEventSettings>(_folder, _type);
        }

        public void SaveChanges()
        {

        }

    }
}
