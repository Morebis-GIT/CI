using System;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;

namespace ImagineCommunications.GamePlan.Domain.ScenarioFailures
{
    public interface IFailuresRepository
    {
        void Add(Failures failures);
        Failures Get(Guid scenarioId);
        void Delete(Guid scenarioId);
        void SaveChanges();

        [Obsolete("Use the Add() method.")]
        void Insert(Failures failures);

        [Obsolete("Use the Delete() method.")]
        void Remove(Guid scenarioId);
    }
}
