using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// CSV audit event repository, persists event to CSV
    /// </summary>
    public class CSVAuditEventRepository : IAuditEventRepository
    {
        private const int MaxValues = 10;

        private static readonly string _delimiter = ((char)9).ToString();

        private readonly string _folder = "";
        private readonly IAuditEventValueTypeRepository _auditEventValueTypeRepository;
        private readonly List<CSVAuditEventSettings> _csvAuditEventSettingsList;
        private readonly List<AuditEventValueConverter> _valueConverters = new List<AuditEventValueConverter>();

        public CSVAuditEventRepository(
            string folder,
            List<AuditEventValueConverter> auditEventValueConverters,
            List<CSVAuditEventSettings> csvAuditEventSettingsList,
            IAuditEventValueTypeRepository auditEventValueTypeRepository)
        {
            _folder = folder;
            _csvAuditEventSettingsList = csvAuditEventSettingsList;
            _auditEventValueTypeRepository = auditEventValueTypeRepository;
            _valueConverters = auditEventValueConverters;
        }

        /// <summary>
        /// Serializes audit event for CSV
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <returns></returns>
        private string SerializeAuditEvent(AuditEvent auditEvent)
        {
            int countValuesDone = 0;
            var values = new StringBuilder();

            foreach (AuditEventValue auditEventValue in auditEvent.Values)
            {
                if (values.Length > 0)
                {
                    _ = values.Append(_delimiter);
                }

                _ = values
                        .Append(auditEventValue.TypeID.ToString())
                        .Append(_delimiter.ToString());

                countValuesDone++;

                string value = SerializeValue(auditEventValue);
                _ = values.Append(value);
            }

            // Add empty values
            for (int valueIndex = countValuesDone; valueIndex < MaxValues; valueIndex++)
            {
                if (values.Length > 0)
                {
                    _ = values.Append(_delimiter);
                }

                _ = values.Append(_delimiter);
            }

            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}", _delimiter, auditEvent.ID, auditEvent.TimeCreated.ToString("o"), auditEvent.TenantID, auditEvent.UserID, auditEvent.Source, auditEvent.EventTypeID, values.ToString());
        }

        public void Insert(AuditEvent auditEvent)
        {
            if (String.IsNullOrEmpty(_folder))
            {
                return;
            }

            if (!Handles(auditEvent))
            {
                return;
            }

            string serializedAuditEvent = SerializeAuditEvent(auditEvent);

            StringBuilder valuesHeader = MakeHeaderRow();

            string file = GetFile(auditEvent.TimeCreated);
            string folder = Path.GetDirectoryName(file);

            if (!Directory.Exists(folder))
            {
                _ = Directory.CreateDirectory(folder);
            }

            bool writeHeaders = !File.Exists(file);

            int retryCount = 3;

            do
            {
                try
                {
                    using TextWriter writer = new StreamWriter(file, true);
                    using var syncTextWriter = TextWriter.Synchronized(writer);

                    try
                    {
                        if (writeHeaders)
                        {
                            WriteHeader(syncTextWriter, valuesHeader);
                        }

                        syncTextWriter.WriteLine(serializedAuditEvent);
                        syncTextWriter.Flush();
                    }
                    finally
                    {
                        syncTextWriter.Close();
                        writer.Close();
                    }

                    retryCount = 0;
                }
                catch (IOException) when (retryCount > 0)
                {
                    Task.Delay(100).Wait();

                    --retryCount;
                }
            } while (retryCount > 0);
        }

        private static void WriteHeader(TextWriter syncTextWriter, StringBuilder valuesHeader)
        {
            syncTextWriter.WriteLine(
                $"ID{_delimiter}Time{_delimiter}TenantID{_delimiter}UserID{_delimiter}Source{_delimiter}EventTypeID{_delimiter}{valuesHeader}"
                );
        }

        private static StringBuilder MakeHeaderRow()
        {
            var valuesHeader = new StringBuilder();

            for (int valueIndex = 0; valueIndex < MaxValues; valueIndex++)
            {
                if (valuesHeader.Length > 0)
                {
                    _ = valuesHeader.Append(_delimiter);
                }

                _ = valuesHeader
                        .Append("ValueType")
                        .Append(valueIndex + 1)
                        .Append(_delimiter)
                        .Append("Value")
                        .Append(valueIndex + 1);
            }

            return valuesHeader;
        }

        private static bool IsExceptionForFileInUse(Exception exception) =>
            exception.Message.Contains("being used by another process");

        private string GetFile(DateTime timeCreated) =>
            Path.Combine(_folder, $"{timeCreated.ToString("dd-MM-yyyy")}.events.txt");

        /// <summary>
        /// Returns the time range for all audit events in the system.
        /// </summary>
        /// <returns></returns>
        private DateTime[] GetAuditEventTimeRangeFromLogs(AuditEventFilter auditEventFilter)
        {
            if (!Directory.Exists(_folder))
            {
                return Array.Empty<DateTime>();
            }

            string[] eventFiles = Directory.GetFiles(_folder, "*.events.txt");
            if (eventFiles.Length == 0)
            {
                return Array.Empty<DateTime>();
            }

            const string dateFormat = "dd-MM-yyyy";
            int dateFormatLength = dateFormat.Length;

            DateTime minDateCreated = DateTime.MinValue.Date;
            DateTime maxDateCreated = DateTime.MaxValue.Date;

            foreach (string file in eventFiles)
            {
                var dateFromFilename = Path.GetFileName(file)
                    .Substring(0, dateFormatLength);

                if (DateTime.TryParseExact(dateFromFilename, dateFormat, null, DateTimeStyles.None, out DateTime date))
                {
                    if (minDateCreated == DateTime.MinValue.Date || date < minDateCreated)
                    {
                        minDateCreated = date;
                    }

                    if (maxDateCreated == DateTime.MaxValue.Date || date > maxDateCreated)
                    {
                        maxDateCreated = date;
                    }
                }
            }

            return new[] { minDateCreated, maxDateCreated };
        }

        /// <summary>
        /// Returns the time range for any run defined in the filter
        /// (e.g. Optimizer report) so that we don't have to search all logs.
        /// If we don't return a range then the system will use the default.
        /// </summary>
        /// <returns></returns>
        private DateTime[] GetAuditEventTimeRangeFromRun(AuditEventFilter auditEventFilter) =>
            Array.Empty<DateTime>();

        /// <summary>
        /// Returns the time range for all audit events in the system
        /// </summary>
        /// <returns></returns>
        private DateTime[] GetAuditEventTimeRange(AuditEventFilter auditEventFilter)
        {
            DateTime[] timeRange = GetAuditEventTimeRangeFromRun(auditEventFilter);
            if (timeRange.Length == 0)
            {
                timeRange = GetAuditEventTimeRangeFromLogs(auditEventFilter);
            }
            return timeRange;
        }

        /// <summary>
        /// Returns events meeting criteria
        /// </summary>
        /// <param name="auditEventFilter"></param>
        /// <returns></returns>
        public List<AuditEvent> Get(AuditEventFilter auditEventFilter)
        {
            List<AuditEvent> auditEvents = new List<AuditEvent>();

            if (String.IsNullOrEmpty(_folder))
            {
                return auditEvents;
            }

            // Get time range of events in system
            DateTime[] timeRange = GetAuditEventTimeRange(auditEventFilter);
            if (timeRange.Length > 0)     // Has events
            {
                // If their range is wider than the events in the system then reduce range so that we check list files
                DateTime minTimeCreated = auditEventFilter.MinTimeCreated.GetValueOrDefault(DateTime.MinValue.Date);
                DateTime maxTimeCreated = auditEventFilter.MaxTimeCreated.GetValueOrDefault(DateTime.MaxValue.Date);
                if (minTimeCreated < timeRange[0])
                {
                    minTimeCreated = timeRange[0];
                }
                if (maxTimeCreated > timeRange[1])
                {
                    maxTimeCreated = timeRange[1];
                }

                minTimeCreated = minTimeCreated.AddDays(-1).Date;
                do
                {
                    minTimeCreated = minTimeCreated.AddDays(1);
                    string auditEventLog = GetFile(minTimeCreated);
                    Get(auditEventFilter, auditEventLog).ForEach(auditEvent => auditEvents.Add(auditEvent));
                } while (minTimeCreated < maxTimeCreated);
            }
            return auditEvents.OrderBy(x => x.TimeCreated).ThenBy(x => x.TenantID).ThenBy(x => x.UserID).ToList();
        }

        /// <summary>
        /// Deletes all events.
        ///
        /// NOTE: At the moment then it deletes ALL events on the dates in the range. It does not respect other filter criteria.
        /// </summary>
        /// <param name="auditEventFilter"></param>
        public void Delete(AuditEventFilter auditEventFilter)
        {
            if (String.IsNullOrEmpty(_folder))
            {
                return;
            }

            // Get time range of events in system
            DateTime[] timeRange = GetAuditEventTimeRange(auditEventFilter);
            if (timeRange.Length > 0)     // Has events
            {
                // If their range is wider than the events in the system then reduce range so that we check list files
                DateTime minTimeCreated = auditEventFilter.MinTimeCreated.GetValueOrDefault(DateTime.MinValue.Date);
                DateTime maxTimeCreated = auditEventFilter.MaxTimeCreated.GetValueOrDefault(DateTime.MaxValue.Date);
                if (minTimeCreated < timeRange[0])
                {
                    minTimeCreated = timeRange[0];
                }
                if (maxTimeCreated > timeRange[1])
                {
                    maxTimeCreated = timeRange[1];
                }

                minTimeCreated = minTimeCreated.AddDays(-1).Date;
                do
                {
                    minTimeCreated = minTimeCreated.AddDays(1);
                    string auditEventLog = GetFile(minTimeCreated);
                    if (File.Exists(auditEventLog))
                    {
                        File.Delete(auditEventLog);
                    }
                } while (minTimeCreated < maxTimeCreated);
            }
        }

        /// <summary>
        /// Returns events meeting criteria from single log file
        /// </summary>
        /// <param name="auditEventFilter"></param>
        /// <param name="auditEventLog"></param>
        /// <returns></returns>
        private List<AuditEvent> Get(AuditEventFilter auditEventFilter, string auditEventLog)
        {
            var auditEvents = new List<AuditEvent>();
            int attempts = 0;

            string[] delimiter = new[] { _delimiter };

            if (File.Exists(auditEventLog))
            {
                do
                {
                    attempts++;
                    try
                    {
                        auditEvents.Clear();
                        using (StreamReader reader = new StreamReader(auditEventLog))
                        {
                            try
                            {
                                string[] headers = new string[0];
                                int lineCount = 0;
                                while (!reader.EndOfStream)
                                {
                                    string[] values = reader.ReadLine().Split(delimiter, StringSplitOptions.None);
                                    lineCount++;
                                    if (lineCount == 1)   // Headers
                                    {
                                        headers = values;
                                    }
                                    else
                                    {
                                        // Deserialise audit event, check if meets criteria. For optimization then DeserializeAuditEvent returns null if the audit
                                        // event header doesn't meet the filter criteria.
                                        try
                                        {
                                            AuditEvent auditEvent = DeserializeAuditEvent(headers, values, auditEventFilter);
                                            if (auditEvent != null)
                                            {
                                                if (IsAuditEventMeetsCriteria(auditEvent, auditEventFilter))
                                                {
                                                    if (!auditEventFilter.IncludeValues)
                                                    {
                                                        auditEvent.Values.Clear();
                                                    }
                                                    auditEvents.Add(auditEvent);
                                                }
                                            }
                                        }
                                        catch { }  // Ignore error
                                    }
                                }
                                attempts = -1;  // Done
                            }
                            catch
                            {
                                throw;
                            }
                            finally
                            {
                                reader.Close();     // Close now, not when GC decides
                            }
                        }
                    }
                    catch (Exception exception) when (IsExceptionForFileInUse(exception) && attempts < 20)
                    {
                        Thread.Sleep(100);
                    }
                } while (attempts != -1);
            }

            return auditEvents.OrderBy(x => x.TimeCreated).ThenBy(x => x.TenantID).ThenBy(x => x.UserID).ToList();
        }

        /// <summary>
        /// Returns whether the audit event header meets the filter criteria
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <param name="auditEventFilter"></param>
        /// <returns></returns>
        private static bool IsAuditEventHeaderMeetsCriteria(AuditEvent auditEvent, AuditEventFilter auditEventFilter)
        {
            if ((auditEvent.TenantID == auditEventFilter.TenantID.GetValueOrDefault(0) || auditEventFilter.TenantID.GetValueOrDefault(0) == 0) &&
               (auditEvent.UserID == auditEventFilter.UserID.GetValueOrDefault(0) || auditEventFilter.UserID.GetValueOrDefault(0) == 0) &&
               (String.IsNullOrEmpty(auditEvent.Source) || String.IsNullOrEmpty(auditEventFilter.Source) || auditEvent.Source.ToUpper() == auditEventFilter.Source.ToUpper()) &&
               (auditEvent.TimeCreated >= auditEventFilter.MinTimeCreated.GetValueOrDefault(DateTime.MinValue.Date)) &&
               (auditEvent.TimeCreated <= auditEventFilter.MaxTimeCreated.GetValueOrDefault(DateTime.MaxValue.Date)) &&
               (auditEventFilter.EventTypeIds == null || auditEventFilter.EventTypeIds.Count == 0 || auditEventFilter.EventTypeIds.Contains(auditEvent.EventTypeID)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns whether audit event meets filter criteria
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <param name="auditEventFilter"></param>
        /// <returns></returns>
        private static bool IsAuditEventMeetsCriteria(AuditEvent auditEvent, AuditEventFilter auditEventFilter)
        {
            if (IsAuditEventHeaderMeetsCriteria(auditEvent, auditEventFilter))      // Header meets criteria
            {
                if (auditEventFilter.ValueFilters.Count == 0)       // No value filters
                {
                    return true;
                }

                // Check value filters
                int countFiltersValid = 0;
                int countFiltersInvalid = 0;
                foreach (AuditEventValueFilter valueFilter in auditEventFilter.ValueFilters)
                {
                    AuditEventValue auditEventValue = auditEvent.GetValueByValueTypeId(valueFilter.ValueTypeID);
                    if (auditEventValue == null)    // Audit event doesn't have value
                    {
                        countFiltersInvalid++;
                    }
                    else     // Has value, check it
                    {
                        if (auditEventValue.Value.Equals(valueFilter.Value))    // Values match
                        {
                            countFiltersValid++;
                        }
                        else
                        {
                            countFiltersInvalid++;
                        }
                    }
                }
                return auditEventFilter.AllFiltersRequired ? (countFiltersValid == auditEventFilter.ValueFilters.Count) : (countFiltersValid > 0);
            }
            return false;
        }

        /// <summary>
        /// <para>
        /// Deserializes AuditEvent from CSV values. As a performance optimization then, if AuditEventFilter is passed, we check that the
        /// header meets the filter criteria and only load the values if it does.
        /// </para>
        /// <para>NOTE: We do not currently deserialize complex objects, just return as serialized string.</para>
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="values"></param>
        /// <param name="auditEventFilter"></param>
        /// <returns></returns>
        private AuditEvent DeserializeAuditEvent(string[] headers, string[] values, AuditEventFilter auditEventFilter = null)
        {
            // Key values by column name so that they're easier to handle
            Dictionary<string, string> valuesKeyed = new Dictionary<string, string>();
            for (int index = 0; index < headers.Length; index++)
            {
                valuesKeyed.Add(headers[index], values[index]);
            }

            // Deserialize
            AuditEvent auditEvent = new AuditEvent()
            {
                ID = valuesKeyed["ID"],
                TimeCreated = DateTimeOffset.Parse(valuesKeyed["Time"]).UtcDateTime,
                TenantID = Convert.ToInt32(valuesKeyed["TenantID"]),
                UserID = Convert.ToInt32(valuesKeyed["UserID"]),
                Source = valuesKeyed.ContainsKey("Source") ? Convert.ToString(valuesKeyed["Source"]) : "",      // New property
                EventTypeID = Convert.ToInt32(valuesKeyed["EventTypeID"])
            };

            // Optimization, return null if header doesn't meet filter criteria, no point in loading values
            if (auditEventFilter != null && !IsAuditEventHeaderMeetsCriteria(auditEvent, auditEventFilter))
            {
                return null;
            }

            // Deserialize values
            for (int valueIndex = 0; valueIndex < MaxValues; valueIndex++)
            {
                string valueTypeColumn = string.Format("ValueType{0}", valueIndex + 1);
                string valueColumn = string.Format("Value{0}", valueIndex + 1);
                if (valuesKeyed.ContainsKey(valueTypeColumn) && !String.IsNullOrEmpty(valuesKeyed[valueTypeColumn]))
                {
                    auditEvent.Values.Add(DeserializeValue(Convert.ToInt32(valuesKeyed[valueTypeColumn]), valuesKeyed[valueColumn]));
                }
            }
            return auditEvent;
        }

        /// <summary>
        /// Serializes value
        /// </summary>
        /// <param name="auditEventValue"></param>
        /// <returns></returns>
        private string SerializeValue(AuditEventValue auditEventValue)
        {
            if (auditEventValue.Value == null)
            {
                return "null";
            }

            AuditEventValueType valueType = _auditEventValueTypeRepository.GetByID(auditEventValue.TypeID);
            AuditEventValueConverter converter = _valueConverters.Find(current => current.Handles(auditEventValue.TypeID));
            if (converter != null)
            {
                return converter.ValueConverter.Convert(auditEventValue.Value, valueType.Type, typeof(String)).ToString();
            }
            return auditEventValue.Value.ToString();
        }

        /// <summary>
        /// Deserializes value
        /// </summary>
        /// <param name="valueTypeId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private AuditEventValue DeserializeValue(int valueTypeId, string value)
        {
            AuditEventValueType valueType = _auditEventValueTypeRepository.GetByID(valueTypeId);
            AuditEventValueConverter converter = _valueConverters.Find(item => item.Handles(valueTypeId));

            var auditEventValue = new AuditEventValue()
            {
                TypeID = Convert.ToInt32(valueTypeId),
                Value = converter == null ? value : converter.ValueConverter.Convert(value, value.GetType(), valueType.Type)
            };

            return auditEventValue;
        }

        private bool Handles(AuditEvent auditEvent)
        {
            if (_csvAuditEventSettingsList == null)     // Repo not implemented yet, log it
            {
                return true;
            }

            CSVAuditEventSettings auditEventSettings = _csvAuditEventSettingsList.Find(aes => aes.EventTypeId == auditEvent.EventTypeID);

            return auditEventSettings != null && auditEventSettings.Enabled;
        }
    }
}
