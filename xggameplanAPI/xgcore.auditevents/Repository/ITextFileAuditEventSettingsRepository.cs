using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    public interface ITextFileAuditEventSettingsRepository
    {
        void Insert(List<TextFileAuditEventSettings> items);

        List<TextFileAuditEventSettings> GetAll();

        void DeleteAll();

        void DeleteByID(int id);

        void Update(TextFileAuditEventSettings item);


        void SaveChanges();
    }
}
