using System;

namespace xggameplan.core.Landmark
{
    public class CancelRunResult
    {
        public Guid ScenarioId { get; set; }
        public Guid ExternalRunId { get; set; }
        public bool IsCancelled { get; set; }
    }
}
