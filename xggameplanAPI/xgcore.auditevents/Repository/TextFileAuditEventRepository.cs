using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Text file audit event repository, writes to text file. This is similar to the CSV version except that this version is intended to be human readable
    /// whereas the CSV version isn't because it needs to be able to events to be able to be queried later.
    /// </summary>
    public class TextFileAuditEventRepository : IAuditEventRepository
    {
        private readonly string _folder;
        private readonly IAuditEventTypeRepository _auditEventTypeRepository;
        private readonly List<ITextFileAuditEventFormatter> _formatters;

        public TextFileAuditEventRepository(IAuditEventTypeRepository auditEventTypeRepository, List<ITextFileAuditEventFormatter> formatters, string folder)
        {
            _auditEventTypeRepository = auditEventTypeRepository;
            _formatters = formatters;
            _folder = folder;
        }

        public void Insert(AuditEvent auditEvent)
        {
            ITextFileAuditEventFormatter formatter = _formatters.FirstOrDefault(f => f.Handles(auditEvent));
            if (formatter == null)  // No formatters
            {
                return;
            }

            string file = GetFile(auditEvent.TimeCreated);
            string folder = Path.GetDirectoryName(file);
            if (!Directory.Exists(folder))
            {
                _ = Directory.CreateDirectory(folder);
            }

            int attempts = 0;
            bool writeHeaders = File.Exists(file);

            // Serialize
            string serializedEvent = formatter.Format(auditEvent);

            do
            {
                try
                {
                    attempts++;
                    using (StreamWriter writer = new StreamWriter(file, true))
                    {
                        try
                        {
                            writer.WriteLine(serializedEvent);
                            writer.Flush();
                            attempts = -1;       // Success
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            writer.Close();     // Close now, not when GC decides
                        }
                    }
                }
                catch (System.Exception exception)
                {
                    if (IsExceptionForFileInUse(exception) && attempts < 20)
                    {
                        System.Threading.Thread.Sleep(100);   // Wait before retry
                    }
                    else
                    {
                        throw;
                    }
                }
            } while (attempts != -1);
        }

        private static bool IsExceptionForFileInUse(Exception exception)
        {
            return exception.Message.Contains("being used by another process");
        }

        private string GetFile(DateTime timeCreated)
        {
            return Path.Combine(_folder, string.Format(@"{0}.events.txt", timeCreated.ToString("dd-MM-yyyy")));
        }

        public List<AuditEvent> Get(AuditEventFilter auditEventFilter)
        {
            throw new NotImplementedException();
        }

        public void Delete(AuditEventFilter auditEventFilter)
        {
            // No action
        }
    }
}
