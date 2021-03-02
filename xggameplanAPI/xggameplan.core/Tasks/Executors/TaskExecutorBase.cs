using System;
using System.Threading;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using xggameplan.AuditEvents;

namespace xggameplan.core.Tasks.Executors
{
    /// <summary>
    /// Base class for task executor
    /// </summary>
    public abstract class TaskExecutorBase
    {
        protected IRepositoryFactory _repositoryFactory;
        protected IAuditEventRepository _auditEventRepository;
        protected Guid _taskInstanceId = Guid.Empty;
        private Thread _thread;
        private volatile bool _active = false;

        /// <summary>
        /// Starts background notification of active status
        /// </summary>
        protected void StartActiveNotifier(Guid taskInstanceId)
        {
            _taskInstanceId = taskInstanceId;
            _active = true;
            if (_thread != null)   // Previous thread active, stop it
            {                
                StopActiveNotifier();                
            }
            _thread = new Thread(ThreadActiveNotifier);
            _thread.IsBackground = true;
            _thread.Start();
        }

        /// <summary>
        /// Stops background notification of active status
        /// </summary>
        protected void StopActiveNotifier()
        {
            try
            {
                _active = false;
                if (_thread != null)
                {
                    _thread.Abort();
                    _thread.Join();
                }
            }
            catch { };  // Ignore
        }

        /// <summary>
        /// Thread for background notification of active status
        /// </summary>
        private void ThreadActiveNotifier()
        {
            try
            {
                while (_active)
                {
                    // Wait
                    for (int index = 0; index < TaskInstance.ActiveFrequencySeconds && _active; index++)
                    {
                        Thread.Sleep(1000);
                    }

                    // Flag active                    
                    try
                    {
                        TaskUtilities.UpdateTaskInstanceStatus(_taskInstanceId, _repositoryFactory, null, null, DateTime.UtcNow);
                    }
                    catch { };      // Ignore                
                }                
            }
            catch(ThreadAbortException)
            {
                // No action
            }            
        }
    }
}
