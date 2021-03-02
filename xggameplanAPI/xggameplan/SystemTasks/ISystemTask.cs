using System.Collections.Generic;

namespace xggameplan.SystemTasks
{
    /// <summary>
    /// Interface for a system task
    /// </summary>
    public interface ISystemTask
    {
        /// <summary>
        /// Task Id
        /// </summary>
        string Id { get; }
 
        /// <summary>
        /// Executes system task
        /// </summary>
        /// <returns></returns>
        List<SystemTaskResult> Execute();        
    }
}
