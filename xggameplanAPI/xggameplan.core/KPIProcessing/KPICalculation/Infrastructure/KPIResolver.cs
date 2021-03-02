using System.Collections.Generic;
using System.Linq;
using Autofac;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;

namespace xggameplan.KPIProcessing.KPICalculation.Infrastructure
{
    /// <inheritdoc />
    public class KPIResolver : IKPIResolver
    {
        private readonly IComponentContext _componentContext;
        private readonly IKPICache _kpiCache;

        public KPIResolver(IComponentContext componentContext, IKPICache kpiCache)
        {
            _componentContext = componentContext;
            _kpiCache = kpiCache;
        }

        /// <inheritdoc />
        public IEnumerable<KPI> Resolve(string kpiName)
        {
            var kpi = _kpiCache.GetKPI(kpiName);

            return kpi != null
                ? new[] {kpi}
                : ResolveInternal(kpiName);
        }

        private IEnumerable<KPI> ResolveInternal(string kpiName)
        {
            var calculator = _componentContext.ResolveKeyed<KPICalculatorBase>(kpiName);
            var dependencies = CheckAndResolveKPIDependencies(calculator);
            var result = calculator.Calculate(dependencies);
            _kpiCache.AddKPIs(result);

            return result;
        }

        private IEnumerable<KPI> CheckAndResolveKPIDependencies(KPICalculatorBase calculator)
        {
            if (calculator.PrerequisiteKPIs is null || !calculator.PrerequisiteKPIs.Any())
            {
                return Enumerable.Empty<KPI>();
            }

            var dependencies = new List<KPI>();

            foreach (var dependencyKpiName in calculator.PrerequisiteKPIs)
            {
                var dependency = _kpiCache.GetKPI(dependencyKpiName);
                if (dependency is null)
                {
                    _ = ResolveInternal(dependencyKpiName);
                    dependency = _kpiCache.GetKPI(dependencyKpiName);
                }

                dependencies.Add(dependency);
            }

            return dependencies;
        }
    }
}
