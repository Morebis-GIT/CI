using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using xggameplan.AuditEvents;

namespace xggameplan.SystemTasks
{
    /// <summary>
    /// Task to delete logs
    /// </summary>
    internal class DeleteLogsTask : ISystemTask
    {
        private string _rootFolder;
        private IAuditEventRepository _auditEventRepository;
        private TimeSpan _retention;

        public DeleteLogsTask(string rootFolder, IAuditEventRepository auditEventRepository, TimeSpan retention)
        {
            _rootFolder = rootFolder;
            _auditEventRepository = auditEventRepository;
            _retention = retention;
        }

        public List<SystemTaskResult> Execute()
        {
            List<SystemTaskResult> results = new List<SystemTaskResult>();

            // Delete log files
            results.AddRange(DeleteLogsByType(_retention, Path.Combine(_rootFolder, "Logs"), "events"));
            results.AddRange(DeleteLogsByType(_retention, Path.Combine(_rootFolder, "Logs"), "requests"));
            results.AddRange(DeleteLogsByType(_retention, Path.Combine(_rootFolder, "Logs"), "responses"));
            results.AddRange(DeleteLogsByType(_retention, Path.Combine(_rootFolder, "Logs"), "smooth"));
            results.AddRange(DeleteLogsByType(_retention, Path.Combine(_rootFolder, "Logs"), "smooth_passes"));
            results.AddRange(DeleteLogsByType(_retention, Path.Combine(_rootFolder, "Logs"), "smooth_configuration"));
            results.AddRange(DeleteLogsByType(_retention, Path.Combine(_rootFolder, "Logs"), "smooth_spot_actions"));
            results.AddRange(DeleteLogsByType(_retention, Path.Combine(_rootFolder, "Logs"), "smooth_best_break"));
            results.AddRange(DeleteLogsByType(_retention, Path.Combine(_rootFolder, "Logs"), "tasks"));                       // xggameplan.taskexecutor.exe
            results.AddRange(DeleteLogsByType(_retention, Path.Combine(_rootFolder, "Scripts", "Tasks", "Logs"), "tasklog"));       // Scheduled tasks
            return results;
        }

        private Dictionary<string, string> FileNamePatternByLogType
        {
            get
            {
                Dictionary<string, string> pattern = new Dictionary<string, string>()
                {
                    { "events", "*.events.txt" },
                    { "requests", "*.requests.txt" },
                    { "responses", "*.responses.txt" },
                    { "smooth", "*.smooth.txt" },
                    { "smooth_passes", "*.smooth_passes.json" },
                    { "smooth_configuration", "*.smooth_configuration.json" },
                    { "smooth_spot_actions", "*.smooth_spot_actions.txt" },
                    { "smooth_best_break", "*.smooth_best_break.txt" },
                    { "tasks", "*.tasks.txt" },
                    { "tasklog", "*.tasklog.txt" }
                };
                return pattern;
            }
        }

        /// <summary>
        /// Deletes all logs of specific type that are older than date range
        ///
        /// Log format: [Date].[Optional:Something].[LogType].txt
        /// </summary>
        /// <param name="retention"></param>
        /// <param name="folder"></param>
        /// <param name="logType"></param>
        /// <returns></returns>
        private List<SystemTaskResult> DeleteLogsByType(TimeSpan retention, string folder, string logType)
        {
            List<SystemTaskResult> results = new List<SystemTaskResult>();
            try
            {
                DateTime minEventDate = DateTime.UtcNow.Subtract(retention);
                const string eventLogDateFormat = "dd-MM-yyyy";
                Dictionary<string, string> fileNamePatternByLogType = FileNamePatternByLogType;
                if (Directory.Exists(folder))
                {
                    foreach (string logFile in GetLogsByType(folder, logType, fileNamePatternByLogType[logType]))
                    {
                        DateTime eventsDate = DateHelper.GetDate(Path.GetFileNameWithoutExtension(logFile), eventLogDateFormat);
                        if (eventsDate < minEventDate)
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, string.Format("Deleting {0} log {1}", logType, Path.GetFileName(logFile))));
                            File.Delete(logFile);
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, string.Format("Deleted {0} log {1}", logType, Path.GetFileName(logFile))));
                        }
                    }
                }
            }
            catch (System.Exception exception)
            {
                results.Add(new SystemTaskResult(SystemTaskResult.ResultTypes.Error, this.Id, string.Format("Error deleting logs for type {0}: {1}", logType, exception.Message)));
            }
            return results;
        }

        /// <summary>
        /// Returns all log files of the specified type from the folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="logType"></param>
        /// <param name="fileNamePattern"></param>
        /// <returns></returns>
        private IEnumerable<string> GetLogsByType(string folder, string logType, string fileNamePattern)
        {
            return Directory.Exists(folder) ? Directory.GetFiles(folder, fileNamePattern) : new string[0];
        }

        public string Id
        {
            get { return "DeleteLogsTask"; }
        }
    }
}
