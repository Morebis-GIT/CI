using System;

namespace xggameplan.SchedulerTasks
{
    /// <summary>
    /// An attribute that can be placed on job to indicate that job should be executed in specific scope.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ScopeJobAttribute : Attribute
    {
        /// <summary>
        /// Gets the scope.
        /// </summary>
        public JobScopeType Scope { get; protected set; }

        /// <summary>
        /// Creates an attribute that can be used to indicate scope specific jobs.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public ScopeJobAttribute(JobScopeType scope)
        {
            Scope = scope;
        }
    }
}
