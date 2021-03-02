namespace xggameplan.SchedulerTasks
{
    /// <summary>
    /// Describes possible scopes of jobs.
    /// </summary>
    public enum JobScopeType
    {
        /// <summary>
        /// The job should be executed in application scope.
        /// </summary>
        /// <remarks>
        /// This is default scope.
        /// </remarks>
        Application,

        /// <summary>
        /// The job should be executed in tenant scope.
        /// </summary>
        Tenant
    }
}
