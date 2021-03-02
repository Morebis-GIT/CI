using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using Microsoft.Extensions.Configuration;
using xggameplan.AuditEvents;

namespace xggameplan.core.Tasks.Executors
{
    /// <summary>
    /// Executes task instance in a separate process. This task executor is different from the others because the caller has the option
    /// of whether to wait for the task to complete.
    /// </summary>
    public class ProcessTaskExecutor : ITaskExecutor
    {
        private readonly string _rootFolder;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IConfiguration _configuration;
        private TimeSpan _completionTimeout;
        private const int defaultTaskInstanceTimeout = 300;

        public ProcessTaskExecutor(string rootFolder,
            IRepositoryFactory repositoryFactory,
            TimeSpan completionTimeout,
            IAuditEventRepository auditEventRepository,
            IConfiguration configuration)
        {
            _rootFolder = rootFolder;
            _repositoryFactory = repositoryFactory;
            _completionTimeout = completionTimeout;
            _auditEventRepository = auditEventRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Starts task execution by starting the process with the TaskInstanceID
        /// </summary>
        /// <param name="taskInstance"></param>
        /// <returns></returns>
        public TaskResult Execute(TaskInstance taskInstance)
        {
            //int taskInstanceTimeout = defaultTaskInstanceTimeout;
            int taskInstanceTimeout;
            if (!int.TryParse(_configuration["Environment:TaskInstanceTimeout"], out taskInstanceTimeout))
            {
                taskInstanceTimeout = defaultTaskInstanceTimeout;
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                    string.Format("Cannot Parse Environment:TaskInstanceTimeout - defaulting to {0} seconds", defaultTaskInstanceTimeout)));
            }
            else
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                     string.Format("Loaded Environment:TaskInstanceTimeout - setting to {0}", taskInstanceTimeout)));
            }

            var waitSpan = TimeSpan.FromSeconds(taskInstanceTimeout);
            StreamReader errorReader = null;
            var taskResult = new TaskResult();
            try
            {
                using var scope = _repositoryFactory.BeginRepositoryScope();
                var taskInstanceRepository = scope.CreateRepository<ITaskInstanceRepository>();

                if (_auditEventRepository != null)
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                        string.Format("Need to execute task {0}: {1} (RootFolder={2})", taskInstance.Id, taskInstance.TaskId, _rootFolder)));
                }

                // Save task instance
                taskInstanceRepository.Add(taskInstance);
                taskInstanceRepository.SaveChanges();

                // Start Task Executor process
                const Char quotes = '"';
                var process = new Process();
                var processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "xggameplan.taskexecutor.exe");
                if (!System.IO.File.Exists(processStartInfo.FileName))  // Not found in same folder as this assembly, try [root]\bin
                {
                    processStartInfo.FileName = string.Format(@"{0}\bin\xggameplan.taskexecutor.exe", _rootFolder);
                }

                if (!System.IO.File.Exists(processStartInfo.FileName))
                {
                    if (_auditEventRepository != null)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                            string.Format("Cannot start process {0} because file does not exist", processStartInfo.FileName)));
                    }
                    throw new System.IO.FileNotFoundException(string.Format("Task Executor executable {0} does not exist", processStartInfo.FileName), processStartInfo.FileName);
                }
                processStartInfo.Arguments = string.Format("/TaskInstanceID={1}{0}{1} /RootFolder={1}{2}{1}", taskInstance.Id, quotes, _rootFolder);
                process.StartInfo = processStartInfo;

                // Start the process, don't wait for it to complete
                if (_auditEventRepository != null)
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                        string.Format("Starting process {0} (Task ID={1})", processStartInfo.FileName, taskInstance.TaskId)));
                }
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;

                bool started = process.Start();
                errorReader = process.StandardOutput;
                if (!started)
                {
                    if (_auditEventRepository != null)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                            string.Format("Failed to start process {0}", processStartInfo.FileName)));
                    }
                    string output = errorReader.ReadToEnd();
                    throw new Exception($"Unable to start Task Executor {processStartInfo.FileName}. Output : {output}");
                }

                // Wait for status to change to indicate that something is happening, process will typically set to InProgress immediately
                if (_auditEventRepository != null)
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                        string.Format("Waiting for status change for process {0} (Task ID={1})", processStartInfo.FileName, taskInstance.TaskId)));
                }

                taskResult.Status = WaitForStatusChange(taskInstance.Id, process, TaskInstanceStatues.Starting, waitSpan);

                if (_auditEventRepository != null)
                {
                    int? exitCode = process.HasExited ? process.ExitCode : (int?)null;
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                        string.Format("Waited (upto) {0} for status change for process {1} (Task ID={2}, (TaskInstance ID={3}, Status={4}, ExitCode={5})",
                        waitSpan.ToString(), processStartInfo.FileName, taskInstance.TaskId, taskInstance.Id, taskResult.Status, (exitCode.HasValue ? exitCode.Value.ToString() : "null"))));
                }

                // Wait for completion if requested
                if (_completionTimeout != null && _completionTimeout.TotalMilliseconds > 0 && taskResult.Status == TaskInstanceStatues.InProgress)
                {
                    if (_auditEventRepository != null)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                            string.Format("Waiting for completion for process {0} (Task ID={1})", processStartInfo.FileName, taskInstance.TaskId)));
                    }
                    taskResult.Status = WaitForCompletion(taskInstance.Id, _completionTimeout);
                    if (_auditEventRepository != null)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                            string.Format("Waited for completion for process {0} (Task ID={1}, Status={2})", processStartInfo.FileName, taskInstance.TaskId, taskResult.Status)));
                    }
                }
            }
            catch (Exception exception)
            {
                if (_auditEventRepository != null)
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                        string.Format("Error executing task for Task ID {0}: {1}", taskInstance.TaskId, exception.Message)));
                }
                string output = errorReader?.ReadToEnd();

                taskResult.Status = TaskInstanceStatues.CompletedError;
                taskResult.Exception = exception;
            }
            return taskResult;
        }

        /// <summary>
        /// Waits for task instance to each completed status or timeout
        /// </summary>
        /// <param name="id"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private TaskInstanceStatues WaitForCompletion(Guid id, TimeSpan timeout)
        {
            DateTime timeoutEnd = DateTime.UtcNow.Add(timeout);
            TaskInstanceStatues status = TaskInstanceStatues.InProgress;
            do
            {
                using var scope = _repositoryFactory.BeginRepositoryScope();
                var taskInstanceRepository = scope.CreateRepository<ITaskInstanceRepository>();
                status = taskInstanceRepository.Get(id).Status;
                if (Array.IndexOf(new TaskInstanceStatues[] { TaskInstanceStatues.CompletedError, TaskInstanceStatues.CompletedSuccess }, status) == -1 && DateTime.UtcNow < timeoutEnd)
                {
                    Thread.Sleep(2000);
                }
                else   // Completed or timeout
                {
                    break;
                }
            } while (true);
            return status;
        }

        /// <summary>
        /// Waits for the task instance status to change or timeout
        /// </summary>
        /// <param name="id"></param>
        /// <param name="process"></param>
        /// <param name="currentStatus"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private TaskInstanceStatues WaitForStatusChange(Guid id, Process process, TaskInstanceStatues currentStatus, TimeSpan timeout)
        {
            var timeNow = DateTime.UtcNow;
            var waitTimeout = timeNow.Add(timeout);

            TaskInstanceStatues newStatus = currentStatus;

            do
            {
                // Wait a bit before we check the status
                if (!process.HasExited)
                {
                    Thread.Sleep(2000);
                }

                using var scope = _repositoryFactory.BeginRepositoryScope();
                var taskInstanceRepository = scope.CreateRepository<ITaskInstanceRepository>();
                TaskInstance taskInstance = taskInstanceRepository.Get(id);
                newStatus = taskInstance.Status;
            } while (currentStatus == newStatus && DateTime.UtcNow < waitTimeout && !process.HasExited);

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                string.Format("WaitForStatusChange [TID: {0}] Waited for {1}", id, DateTime.UtcNow - timeNow)));

            return newStatus;
        }
    }
}
