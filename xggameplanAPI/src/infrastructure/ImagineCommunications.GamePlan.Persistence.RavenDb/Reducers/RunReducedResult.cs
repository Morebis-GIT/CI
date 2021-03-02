using System;
using ImagineCommunications.GamePlan.Domain.Runs;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Reducers
{
    public class RunReducedResult
    {
        public string Query { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public RunStatus RunStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime? ExecuteStartedDateTime { get; set; }
        public string Description { get; set; }
    }
}
