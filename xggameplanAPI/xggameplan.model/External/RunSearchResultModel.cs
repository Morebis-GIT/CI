using System;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using xggameplan.Model;

namespace xggameplan.model.External
{
    public class RunSearchResultModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ExecuteStartedDateTime { get; set; }
        public AuthorModel Author { get; set; }
        public RunStatus Status { get; set; }
        public bool HasNonPendingScenario { get; set; }
        public bool HasAllScenarioCompletedSuccessfully { get; set; }
        public DateTime? FirstScenarioStartedDateTime { get; set; }
        public DateTime? LastScenarioCompletedDateTime { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public InventoryLockModel InventoryLock { get; set; }
        public ExternalScenarioStatus? ExternalStatus { get; set; }
    }
}
