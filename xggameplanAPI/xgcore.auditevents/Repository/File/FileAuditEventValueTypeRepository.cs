using System.Collections.Generic;
using xgcore.auditevents.Repository.File;
using xggameplan.AuditEvents;

namespace xggameplan.Repository.File
{
    /// <summary>
    /// Repository for AuditEventValueType instances (File)
    /// </summary>
    public class FileAuditEventValueTypeRepository : FileRepositoryBase, IAuditEventValueTypeRepository
    {
        public FileAuditEventValueTypeRepository(string folder) : base(folder, "audit_event_value_type")
        {
        }

        public void Insert(List<AuditEventValueType> items)
        {
            InsertItems(_folder, _type, items, items.ConvertAll(i => i.ID.ToString()));
        }

        public void Update(AuditEventValueType item)
        {
            UpdateOrInsertItem(_folder, _type, item, item.ID.ToString());
        }

        public void DeleteByID(int id)
        {
            DeleteItem(_folder, _type, id.ToString());
        }

        public List<AuditEventValueType> GetAll() =>
            GetAllByType<AuditEventValueType>(_folder, _type);

        public AuditEventValueType GetByID(int id)
        {
            return GetItemByID<AuditEventValueType>(_folder, _type, id.ToString());
        }

        public void DeleteAll()
        {
            DeleteAllItems(_folder, _type);
        }
    }
}
