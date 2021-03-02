using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Interface for File audit event settings repository
    /// </summary>
    public interface IFileAuditEventSettingsRepository
    {
        void Insert(List<FileAuditEventSettings> items);

        List<FileAuditEventSettings> GetAll();

        void DeleteAll();

        void DeleteByID(int id);

        void Update(FileAuditEventSettings item);


        void SaveChanges();
    }
}
