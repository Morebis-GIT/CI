using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;

namespace xggameplan.KPIProcessing.Abstractions
{
    public abstract class KPICalculatorBase
    {
        private Dictionary<string, KPI> _resolvedDependencies = null;

        public IEnumerable<string> PrerequisiteKPIs => DependenciesDefinition;
        protected virtual HashSet<string> DependenciesDefinition { get; } = null;

        public IReadOnlyCollection<KPI> Calculate(IEnumerable<KPI> dependencies)
        {
            if (DependenciesDefinition is null || DependenciesDefinition.Count == 0)
            {
                return Calculate();
            }

            _resolvedDependencies = new Dictionary<string, KPI>();
            var unresolvedDependenciesCount = DependenciesDefinition.Count;

            foreach (var kpi in dependencies)
            {
                if (!DependenciesDefinition.Contains(kpi.Name))
                {
                    continue;
                }

                unresolvedDependenciesCount--;
                _resolvedDependencies.Add(kpi.Name, kpi);
            }

            if (unresolvedDependenciesCount > 0)
            {
                throw new ArgumentException($"not all dependencies resolved for: {GetType().Name}", nameof(dependencies));
            }

            return Calculate();
        }

        protected abstract IReadOnlyCollection<KPI> Calculate();

        protected KPI GetDependencyValue(string name) => _resolvedDependencies[name];
    }
}
