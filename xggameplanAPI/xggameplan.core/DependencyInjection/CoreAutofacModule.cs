using System.Runtime.Caching;
using Autofac;
using ImagineCommunications.Gameplan.Synchronization;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.DomainLogic;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Process.Smooth;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using xggameplan.common.Email;
using xggameplan.common.Types;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.AutoBooks.DataHandlers;
using xggameplan.core.BRS;
using xggameplan.core.FeatureManagement;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Interfaces;
using xggameplan.core.ReportGenerators.Interfaces;
using xggameplan.core.ReportGenerators.ScenarioCampaignResults;
using xggameplan.core.RunManagement;
using xggameplan.core.RunManagement.BreakAvailabilityCalculator;
using xggameplan.core.RunManagement.Notifications;
using xggameplan.core.Services;
using xggameplan.core.Services.OptimiserInputFilesSerialisers;
using xggameplan.core.Services.OptimiserInputFilesSerialisers.Breaks;
using xggameplan.core.Services.OptimiserInputFilesSerialisers.Campaigns;
using xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces;
using xggameplan.core.Services.RunCleaning;
using xggameplan.core.Validators;
using xggameplan.core.Validators.ProductAdvertiser;
using xggameplan.Email;
using xggameplan.FeatureManagement;
using xggameplan.KPIProcessing;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;
using xggameplan.Repository;
using xggameplan.Repository.Memory;
using xggameplan.RunManagement;
using xggameplan.RunManagement.Notifications;
using xggameplan.Services;

namespace xggameplan.core.DependencyInjection
{
    /// <summary>
    /// Common tenant's registrations
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class CoreAutofacModule : Module
    {
        private readonly string _rootFolder;

        public CoreAutofacModule(string rootFolder)
        {
            _rootFolder = rootFolder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            _ = builder.RegisterInstance(new RootFolder(_rootFolder)).AsSelf();

            _ = builder.Register(context =>
            {
                return new FeatureManager(
                    context.Resolve<TenantIdentifier>(),
                    context.Resolve<IFeatureSettingsProvider>());
            }).As<IFeatureManager>().InstancePerLifetimeScope();

            _ = builder.RegisterType<KPIResolver>().As<IKPIResolver>().InstancePerLifetimeScope();
            _ = builder.RegisterType<KPICalculationManager>().As<IKPICalculationManager>().InstancePerLifetimeScope();
            _ = builder.RegisterType<KPICalculationScopeFactory>().As<IKPICalculationScopeFactory>();
            _ = builder.RegisterType<ScenarioCampaignMetricsProcessor>().As<IScenarioCampaignMetricsProcessor>();
            _ = builder.RegisterType<AnalysisGroupKPIsCalculator>().As<IAnalysisGroupKPIsCalculator>();
            builder.RegisterKPICalculators();

            // Synchronization service
            _ = builder.RegisterType<SynchronizationService>().AsSelf().InstancePerLifetimeScope();
            _ = builder
                .Register(x =>
                    x.Resolve<IFeatureManager>().IsEnabled(nameof(ProductFeature.IntegrationSynchronization))
                        ? (ISynchronizationService)x.Resolve<SynchronizationService>()
                        : new EmptySynchronizationService())
                .As<ISynchronizationService>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterInstance(new SynchronizationServicesConfiguration()
                .Add(SynchronizedServiceType.RunExecution)
                .Add(SynchronizedServiceType.DataSynchronization, maxConcurrencyLevel: 1));

            _ = builder.Register(context => EmailUtilities.GetEmailConnection(context.Resolve<IConfiguration>()))
                .As<IEmailConnection>().InstancePerLifetimeScope();

            _ = builder.Register(
                    x =>
                    {
                        var tenantSettingsRepository = x.Resolve<ITenantSettingsRepository>();
                        if (tenantSettingsRepository is null)
                        {
                            return ClashExposureCountService.Create();
                        }

                        TenantSettings tenantSettings = tenantSettingsRepository.Get();
                        if (tenantSettings is null)
                        {
                            return ClashExposureCountService.Create();
                        }

                        var peakStartAndEnd = (tenantSettings.PeakStartTime, tenantSettings.PeakEndTime);

                        return ClashExposureCountService.Create(peakStartAndEnd);
                    })
                .As<IClashExposureCountService>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<DataChangeValidator>().As<IDataChangeValidator>().InstancePerLifetimeScope();
            _ = builder.RegisterType<RepositoryFactory>().As<IRepositoryFactory>().InstancePerLifetimeScope();

            _ = builder.Register(context =>
            {
                var nc = new NotificationCollection();
                nc.Add(new HTTPTNotification());
                return nc;
            })
                .As<INotificationCollection>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<OptimiserInputFiles>().InstancePerLifetimeScope();
            _ = builder.RegisterType<DataManipulator>().As<IDataManipulator>().InstancePerLifetimeScope();
            _ = builder.RegisterType<ModelLoaders>().As<IModelLoaders>().InstancePerLifetimeScope();

            // Register Smooth processor
            _ = builder.RegisterType<SmoothEngine>().AsSelf();
            _ = builder.RegisterType<SmoothProcessor>().As<ISmoothProcessor>();

            // Register FeatureManager
            _ = builder.RegisterType<TenantSettingsFeatureManager>().As<ITenantSettingsFeatureManager>();

            _ = builder.RegisterType<AutoBookInputHandler>().As<IAutoBookInputHandler>();
            _ = builder.RegisterType<AutoBookOutputHandler>().As<IAutoBookOutputHandler>();

            _ = builder.RegisterType<BRSIndicatorManager>().As<IBRSIndicatorManager>().InstancePerDependency();
            _ = builder.RegisterType<BRSCalculator>().As<IBRSCalculator>().InstancePerDependency();

            // Register RunCleaner
            _ = builder.RegisterType<RunCleaner>().As<IRunCleaner>().InstancePerLifetimeScope();
            // Register RunManager
            _ = builder.RegisterType<RunManager>().As<IRunManager>().InstancePerDependency();

            //Register RecalculateBreakAvailabilityService
            _ = builder.RegisterType<RecalculateBreakAvailabilityService>().As<IRecalculateBreakAvailabilityService>().InstancePerDependency();

            _ = builder.RegisterType<SystemLogicalDateService>().As<ISystemLogicalDateService>();

            _ = builder.Register(context => LoggerFactory.Create(config => config.AddDebug()))
                .Named<ILoggerFactory>("Debug")
                .SingleInstance();

            _ = builder.Register(context => context.ResolveNamed<ILoggerFactory>("Debug").CreateLogger<IRecalculateBreakAvailabilityService>())
                .As<ILogger<IRecalculateBreakAvailabilityService>>()
                .InstancePerDependency();

            _ = builder.RegisterType<ScenarioSnapshotGenerator>().AsSelf();

            _ = builder.RegisterType<RunCompletionNotifier>().AsSelf().InstancePerLifetimeScope();
            _ = builder.RegisterType<RunInstanceCreator>().AsSelf().InstancePerLifetimeScope();
            _ = builder.RegisterType<RunScenarioTask>().AsSelf().InstancePerLifetimeScope();
            _ = builder.RegisterType<RunScenarioTaskExecutor>().AsSelf().InstancePerLifetimeScope();

            _ = builder.Register<ISystemMessageRepository>(x => new SystemMessageRepository());

            _ = builder.RegisterType<IdentityGeneratorResolver>()
                .As<IIdentityGeneratorResolver>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterType<ProductAdvertiserValidator>().As<IProductAdvertiserValidator>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterType<ClashExceptionSerializer>().As<IClashExceptionSerializer>().InstancePerLifetimeScope();
            _ = builder.RegisterType<CampaignSerializer>().As<ICampaignSerializer>().InstancePerLifetimeScope();
            _ = builder.RegisterType<BreakSerializer>().As<IBreakSerializer>().InstancePerLifetimeScope();
            _ = builder.RegisterType<CampaignFlattener>().As<ICampaignFlattener>().InstancePerLifetimeScope();
            _ = builder.RegisterType<CampaignPassPrioritiesService>().As<ICampaignPassPrioritiesService>().InstancePerLifetimeScope();
            _ = builder.RegisterType<PassInspectorService>().As<IPassInspectorService>().InstancePerLifetimeScope();
            _ = builder.RegisterType<AnalysisGroupCampaignQuery>().As<IAnalysisGroupCampaignQuery>().InstancePerLifetimeScope();
            _ = builder.RegisterType<RecommendationAggregator>().As<IRecommendationAggregator>().InstancePerLifetimeScope();
            _ = builder.RegisterType<ScenarioCampaignResultReportCreator>().As<IScenarioCampaignResultReportCreator>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterType<RecommendationsResultReportCreator>().As<IRecommendationsResultReportCreator>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterType<SpotModelCreator>().As<ISpotModelCreator>()
                .InstancePerLifetimeScope();
            _ = builder.RegisterType<ProgTxDetailSerializer>().As<IProgTxDetailSerializer>().InstancePerLifetimeScope();

            _ = builder.Register(x => MemoryCache.Default).As<MemoryCache>().SingleInstance();

            _ = builder.RegisterType<CampaignCleaner>().As<ICampaignCleaner>().InstancePerLifetimeScope();
        }
    }
}
