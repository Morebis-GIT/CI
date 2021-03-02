using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// File audit event repository
    /// </summary>
    public class FileAuditEventRepository : IAuditEventRepository
    {
        private string _folder;
        private List<FileAuditEventSettings> _fileAuditEventSettingsList;

        public FileAuditEventRepository(string folder, List<FileAuditEventSettings> fileAuditEventSettingsList)
        {
            _folder = folder;
            _fileAuditEventSettingsList = fileAuditEventSettingsList;
        }

        public void Insert(AuditEvent auditEvent)
        {
            if (!Handles(auditEvent))
            {
                return;
            }
            UpdateOrInsertItem<AuditEvent>(_folder, "audit_event", auditEvent, auditEvent.ID);
        }

        public List<AuditEvent> Get(AuditEventFilter auditEventFilter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes the object to byte array for content body of request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        private byte[] SerializeContentBody<T>(T item)
        {
            string contentString = JsonConvert.SerializeObject(item, Formatting.Indented);
            return Encoding.UTF8.GetBytes(contentString);
        }

        /// <summary>
        /// Deserializes content body string to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentString"></param>
        /// <returns></returns>
        private T DeserializeContentBody<T>(string contentString)
        {
            T item = JsonConvert.DeserializeObject<T>(contentString);
            return item;
        }

        private void UpdateOrInsertItem<T>(string folder, string type, T item, string id)
        {
            string file = System.IO.Path.Combine(folder, string.Format(@"{0}.{1}.json", id, type));
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
            System.IO.File.WriteAllBytes(file, SerializeContentBody<T>(item));
        }

        public void Delete(AuditEventFilter auditEventFilter)
        {
            // No action
        }

        private bool Handles(AuditEvent auditEvent)
        {
            FileAuditEventSettings auditEventSettings = _fileAuditEventSettingsList.Find(aes => aes.EventTypeId == auditEvent.EventTypeID);
            return (auditEventSettings != null && auditEventSettings.Enabled);
        }
    }
}
