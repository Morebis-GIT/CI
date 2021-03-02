using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.AutoBook
{
    public class AutoBookTaskModel
    {
        public Guid RunId { get; set; }

        public Guid ScenarioId { get; set; }

        public override bool Equals(object obj) =>
            obj is AutoBookTaskModel model &&
            RunId.Equals(model.RunId) &&
            ScenarioId.Equals(model.ScenarioId);

        public override int GetHashCode() => 0;
    }
}
