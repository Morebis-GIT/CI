using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;

namespace ImagineCommunications.GamePlan.Domain.ScenarioResults
{
    public interface IScenarioResultRepository
    {
        ScenarioResult Find(Guid scenarioId);

        IEnumerable<ScenarioResult> Find(Guid[] scenarioIds);

        IEnumerable<ScenarioResult> GetAll();

        void Add(ScenarioResult scenarioResult);

        void Update(ScenarioResult scenarioResult);

        void UpdateRange(IEnumerable<ScenarioResult> items);

        void Remove(Guid id);

        void SaveChanges();
    }
}
