using System.Collections.Generic;
using xggameplan.RunManagement;

namespace xggameplan.SystemTasks
{
    /// <summary>
    /// Handles crashed runs, runs that are stuck in a particular state and will not recover
    /// </summary>
    internal class CrashedRunsTask : ISystemTask
    {
        private readonly IRunManager _runManager;

        public CrashedRunsTask(IRunManager runManager)
        {
            _runManager = runManager;
        }

        public List<SystemTaskResult> Execute()
        {
            List<SystemTaskResult> results = new List<SystemTaskResult>();

            var crashedRuns = _runManager.HandleCrashedRuns(); ;

            foreach(var run in crashedRuns)
            {
                results.Add(new SystemTaskResult(SystemTaskResult.ResultTypes.Warning, this.Id, string.Format("Run {0} crashed", run.Description)));
            }            
            return results;
        }      

        public string Id
        {
            get { return "CrashedRunsTask"; }
        }
    }
}
