using Autofac;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using xggameplan.core.OutputProcessors.DataHandlers;
using xggameplan.core.OutputProcessors.Processors;
using xggameplan.OutputFiles.Processing;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.OutputProcessors.Processors;

namespace xggameplan.taskexecutor.DependencyInjection
{
    public class OutputFilesProcessingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            _ = builder.RegisterType<BaseRatingsOutputFileProcessor>().As<IOutputFileProcessor<BaseRatingsOutput>>().InstancePerLifetimeScope();
            _ = builder.RegisterType<BreakEfficiencyOutputFileProcessor>().As<IOutputFileProcessor<ProcessBreakEfficiencyOutput>>().InstancePerLifetimeScope();
            _ = builder.RegisterType<CampaignsReqmOutputFileProcessor>().As<IOutputFileProcessor<CampaignsReqmOutput>>().InstancePerLifetimeScope();
            _ = builder.RegisterType<ConversionEfficiencyOutputFileProcessor>().As<IOutputFileProcessor<ConversionEfficiencyOutput>>();
            _ = builder.RegisterType<FailuresFileProcessor>().As<IOutputFileProcessor<Failures>>().InstancePerLifetimeScope();
            _ = builder.RegisterType<ReserveRatingsOutputFileProcessor>().As<IOutputFileProcessor<ReserveRatingsOutput>>().InstancePerLifetimeScope();
            _ = builder.RegisterType<ScenarioCampaignFailureOutputFileProcessor>().As<IOutputFileProcessor<ScenarioCampaignFailureOutput>>().InstancePerLifetimeScope();
            _ = builder.RegisterType<ScenarioCampaignResultsFileProcessor>().As<IOutputFileProcessor<ScenarioCampaignResult>>().InstancePerLifetimeScope();
            _ = builder.RegisterType<ScenarioCampaignLevelResultsFileProcessor>().As<IOutputFileProcessor<ScenarioCampaignLevelResult>>().InstancePerLifetimeScope();
            _ = builder.RegisterType<SpotsReqmOutputFileProcessor>().As<IOutputFileProcessor<SpotsReqmOutput>>().InstancePerLifetimeScope();

            _ = builder.RegisterType<FailuresDataHandler>().As<IOutputDataHandler<Failures>>().InstancePerLifetimeScope();
            _ = builder.RegisterType<ScenarioCampaignFailuresDataHandler>().As<IOutputDataHandler<ScenarioCampaignFailureOutput>>().InstancePerLifetimeScope();
            _ = builder.RegisterType<ScenarioCampaignResultsDataHandler>().As<IOutputDataHandler<ScenarioCampaignResult>>().InstancePerLifetimeScope();
            _ = builder.RegisterType<ScenarioCampaignLevelResultsDataHandler>().As<IOutputDataHandler<ScenarioCampaignLevelResult>>().InstancePerLifetimeScope();
            _ = builder.RegisterType<SpotsReqmOutputDataHandler>().As<IOutputDataHandler<SpotsReqmOutput>>().InstancePerLifetimeScope();
        }
    }
}
