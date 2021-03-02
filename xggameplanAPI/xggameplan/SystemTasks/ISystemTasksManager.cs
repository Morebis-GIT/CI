using System.Collections.Generic;

namespace xggameplan.SystemTasks
{
    /// <summary>
    /// Manages system tasks
    /// </summary>
    public interface ISystemTasksManager
    {
        bool TaskExists(string taskId);

        /// <summary>
        /// Executes single task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        List<SystemTaskResult> ExecuteTask(string taskId);

     
    }
}
