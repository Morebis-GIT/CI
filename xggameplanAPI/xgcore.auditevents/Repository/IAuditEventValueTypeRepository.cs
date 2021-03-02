using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    public interface IAuditEventValueTypeRepository
    {
        void Insert(List<AuditEventValueType> items);

        void DeleteByID(int id);

        void Update(AuditEventValueType item);

        List<AuditEventValueType> GetAll();

        AuditEventValueType GetByID(int it);

        void DeleteAll();
    }
}
