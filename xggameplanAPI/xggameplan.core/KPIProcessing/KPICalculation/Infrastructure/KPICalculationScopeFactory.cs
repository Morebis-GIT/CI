using System;
using Autofac;
using xggameplan.core.OutputProcessors;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.Model;
using xggameplan.OutputProcessors.Abstractions;

namespace xggameplan.KPIProcessing.KPICalculation.Infrastructure
{
    /// <inheritdoc />
    public class KPICalculationScopeFactory : IKPICalculationScopeFactory
    {
        private readonly ILifetimeScope _lifetimeScope;

        public KPICalculationScopeFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        /// <inheritdoc />
        public IKPICalculationScope CreateCalculationScope(Guid runId, Guid scenarioId) =>
            new KPICalculationScope(_lifetimeScope.BeginLifetimeScope(builder =>
            {
                _ = builder.RegisterInstance(new RunWithScenarioReference(runId, scenarioId)).SingleInstance();

                _ = builder.RegisterType<OutputImmutableDataSnapshot>()
                    .As<IOutputDataSnapshot>()
                    .SingleInstance();

                _ = builder.RegisterType<KPICalculationContext>()
                    .As<IKPICalculationContext>()
                    .As<IKPICache>()
                    .SingleInstance();
            }));
    }
}
