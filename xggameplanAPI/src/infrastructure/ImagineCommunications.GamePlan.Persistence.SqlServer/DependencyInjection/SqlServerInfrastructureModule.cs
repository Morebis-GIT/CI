using Autofac;
using ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Fts;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection
{
    public class SqlServerInfrastructureModule : SqlServerAutofacModuleBase
    {
        protected override void Load(ContainerBuilder builder)
        {
            _ = builder.RegisterType<FullTextSearchConditionBuilder>().As<IFullTextSearchConditionBuilder>()
                    .InstancePerDependency();

            //TODO: it might be read from configuration (if necessary)
            _ = builder.Register(_ => new RecalculateBreakAvailabilityOptions
            {
                BoundedCalculateTaskCapacity = 200,
                BoundedDbContextOperationCapacity = 40,
                UpdateBreakDegreeOfParallelism = 3,
                UpdateBreakBatchSize = 3_000
            }).AsSelf().InstancePerDependency();
        }
    }
}
