using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    public interface IAuditEventTypeRepository
    {
        void Insert(List<AuditEventType> items);

        void DeleteByID(int id);

        void Update(AuditEventType item);

        List<AuditEventType> GetAll();

        AuditEventType GetByID(int id);

        void DeleteAll();
    }
}
