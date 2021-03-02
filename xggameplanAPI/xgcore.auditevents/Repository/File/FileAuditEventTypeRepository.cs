using System.Collections.Generic;
using System.Linq;
using xgcore.auditevents.Repository.File;
using xggameplan.AuditEvents;

namespace xggameplan.Repository.File
{
    /// <summary>
    /// Repository for AuditEventType instances (File)
    /// </summary>
    public class FileAuditEventTypeRepository
        : FileRepositoryBase, IAuditEventTypeRepository
    {
        public FileAuditEventTypeRepository(string folder)
            : base(folder, "audit_event_type") { }

        public void Insert(List<AuditEventType> items) =>
            InsertItems(_folder, _type, items, items.Select(i => i.ID.ToString()).ToList());

        public void Update(AuditEventType item) =>
            UpdateOrInsertItem<AuditEventType>(_folder, _type, item, item.ID.ToString());

        public void DeleteByID(int id) =>
            DeleteItem<AuditEventType>(_folder, _type, id.ToString());

        public List<AuditEventType> GetAll() =>
            GetAllByType<AuditEventType>(_folder, _type);

        public AuditEventType GetByID(int id) =>
            GetItemByID<AuditEventType>(_folder, _type, id.ToString());

        public void DeleteAll() =>
            DeleteAllItems<AuditEventType>(_folder, _type);
    }
}
