using System;

namespace xggameplan.Model
{
    /// <summary>
    /// Model to start task
    /// </summary>
    public class StartAutoBookTaskModel
    {
        public Guid runId { get; set; }
        public Guid scenarioId { get; set; }
        public string binariesVersion { get; set; }
    }
}
