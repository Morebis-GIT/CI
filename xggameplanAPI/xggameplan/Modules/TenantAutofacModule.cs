using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Data.Installers;
using ImagineCommunications.Gameplan.Synchronization.SqlServer;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared;
using ImagineCommunications.GamePlan.Domain.Shared.System;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb.DependencyInjection;
using ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Serilog;
using Serilog.Extensions.Logging;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.AutoBooks.AWS;
using xggameplan.common.BackgroundJobs;
using xggameplan.common.Extensions;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Hubs;
using xggameplan.core.Interfaces;
using xggameplan.core.Landmark;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.core.Landmark.PayloadProviders;
using xggameplan.core.Logging;
using xggameplan.core.Services;
using xggameplan.Helpers;
using xggameplan.Hubs;
using xggameplan.Jobs;
using xggameplan.Logging;
using xggameplan.model.Internal;
using xggameplan.PersistenceAttributes;
using xggameplan.SchedulerTasks.Jobs;
using xggameplan.Services;
using xggameplan.SystemTasks;
using xggameplan.SystemTests;
using xggameplan.TestEnv;
using xggameplan.TestEnv.AutoBook;

namespace xggameplan.Modules
{
    /// <summary>
    /// Raven Tenant Autofac module.
    /// </summary>
    public class TenantAutofacModule : Module
    {
        private readonly Tenant _tenant;
        private readonly IConfiguration _applicationConfiguration;
        private readonly ITestEnvironment _testEnvironment;
        private readonly IFeatureManager _featureManager;
        private readonly string _autobookProviderApiAccessToken;

        public TenantAutofacModule(Tenant tenant, IConfiguration applicationConfiguration,
            ITestEnvironment testEnvironment, IFeatureManager featureManager)
        {
            _tenant = tenant;
            _applicationConfiguration = applicationConfiguration;
            _testEnvironment = testEnvironment;
            _featureManager = featureManager;
            _autobookProviderApiAccessToken = applicationConfiguration["AutoBooks:AutobookProviderApiAccessToken"];
        }

        protected override void Load(ContainerBuilder builder)
        {
            _ = builder.RegisterInstance(new TenantIdentifier(_tenant.Id, _tenant.Name)).AsSelf().SingleInstance();

            _ = builder.RegisterType<HostingBackgroundJobManager>()
                .As<IBackgroundJobManager>()
                .SingleInstance();

            var tenantConnectionString = _applicationConfiguration.IsTempTenantDefined()
                ? _applicationConfiguration.GetTempTenantConnectionString()
                : _tenant.TenantDb?.ConnectionString;
            switch (_applicationConfiguration.GetDbProvider())
            {
                case DbProviderType.RavenDb:
                    _ = builder.RegisterModule(new RavenDbTenantModule(tenantConnectionString));
                    _ = builder.RegisterType<RavenSessionManagementAttribute>()
                        .AsWebApiActionFilterFor<ApiController>()
                        .InstancePerRequest();
                    break;

                case DbProviderType.SqlServer:
                    var loggingEnabled = _testEnvironment.HasOptions(TestEnvironmentOptions.PerformanceLog);
                    var longRunningCommandsTimeout = _applicationConfiguration.GetValue<int?>("SqlServerDb:LongRunningCommandsTimeout");
                    _ = builder.RegisterModule(new SqlServerTenantModule(tenantConnectionString,
                        new SqlServerDbContextRegistrationFeatures { Logging = loggingEnabled },
                        new SqlServerDbContextRegistrationFeatures { Logging = loggingEnabled, CommandTimeout = longRunningCommandsTimeout ?? 600 }));
                    _ = builder.RegisterModule(new SqlServerFeatureManagementModule(_featureManager));
                    _ = builder.RegisterModule(new SynchronizationServiceSqlServerServiceModule(tenantConnectionString));
                    _ = builder.RegisterModule(new IntelligenceAutofacModule(tenantConnectionString));
                    break;
            }

            // Register System Tests Manager
            _ = builder.RegisterType<SystemTestsManager>().As<ISystemTestsManager>();

            // Register System Tasks Manager
            _ = builder.RegisterType<SystemTasksManager>().As<ISystemTasksManager>();

            if (!_testEnvironment.HasOptions(TestEnvironmentOptions.AutoBookStub))
            {
                _ = builder.Register(x =>
                {
                    var autoBookSettingsRepository = x.Resolve<IAutoBookSettingsRepository>();
                    var autoBookSettings = autoBookSettingsRepository.Get();

                    // TODO: Must be seeded once on application start
                    if (autoBookSettings == null)       // AutoBook settings not found, add default settings
                    {
                        autoBookSettings = AutoBookHelper.GetDefaultAutoBookSettings();
                        autoBookSettingsRepository.AddOrUpdate(autoBookSettings);
                        autoBookSettingsRepository.SaveChanges();
                    }

                    return autoBookSettings;
                }).InstancePerLifetimeScope();

                _ = builder.Register(x =>
                {
                    var autoBookSettings = x.Resolve<AutoBookSettings>();
                    var auditEventRepository = x.Resolve<IAuditEventRepository>();
                    return new AWSAutoBooksAPI(autoBookSettings.ProvisioningAPIURL, _autobookProviderApiAccessToken, auditEventRepository);
                })
                    .As<IAutoBooksAPI<AWSPAAutoBook, AWSPACreateAutoBook>>()
                    .InstancePerDependency();

                // Register AutoBooks Management for collectively managing AutoBooks
                _ = builder.Register(x =>
                {
                    var autoBookRepository = x.Resolve<IAutoBookRepository>();
                    var autoBookTypeRepository = x.Resolve<IAutoBookInstanceConfigurationRepository>();
                    var auditEventRepository = x.Resolve<IAuditEventRepository>();
                    var awsInstanceConfiguration = x.Resolve<IAWSInstanceConfigurationRepository>();
                    var autoBookSettings = x.Resolve<AutoBookSettings>();
                    var repositoryFactory = x.Resolve<IRepositoryFactory>();
                    var autoBooksApi = x.Resolve<IAutoBooksAPI<AWSPAAutoBook, AWSPACreateAutoBook>>();

                    return new AWSAutoBooks(repositoryFactory, autoBookRepository, autoBookTypeRepository,
                        awsInstanceConfiguration,
                        auditEventRepository, autoBookSettings, autoBooksApi, _autobookProviderApiAccessToken);
                })
                    .As<IAutoBooks>()
                    .InstancePerDependency();
            }
            else
            {
                _ = builder.Register(x =>
                {
                    var autoBookSettingsRepository = x.Resolve<IAutoBookSettingsRepository>();
                    var autoBookSettings = autoBookSettingsRepository.Get();

                    // TODO: Must be seeded once on application start
                    if (autoBookSettings == null)
                    {
                        autoBookSettings = AutoBookHelper.GetDefaultAutoBookSettings();
                        autoBookSettingsRepository.AddOrUpdate(autoBookSettings);
                        autoBookSettingsRepository.SaveChanges();
                    }

                    var returnAutoBookSettings = AutoBookHelper.GetDefaultAutoBookSettings();
                    returnAutoBookSettings.MaxInstances = System.Int32.MaxValue;
                    returnAutoBookSettings.CreationTimeout = Duration.Zero;
                    returnAutoBookSettings.ApplicationVersion = "v1-test-env";

                    return returnAutoBookSettings;
                }).InstancePerLifetimeScope();

                _ = builder.RegisterType<AWSAutoBooksAPIStub>()
                    .As<IAutoBooksAPI<AWSPAAutoBook, AWSPACreateAutoBook>>()
                    .As<IAutoBooksTestHandler>()
                    .InstancePerDependency();

                _ = builder.RegisterType<TestEnvironmentAWSAutoBooks>().As<IAutoBooks>().InstancePerDependency();
            }

            _ = builder.RegisterType<TestEnvironmentDataService>()
                .As<ITestEnvironmentDataService>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<HubNotification<RunNotificationHub, RunNotification>>()
                .As<IHubNotification<RunNotification>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<HubNotification<LandmarkRunStatusNotificationHub, LandmarkRunStatusNotification>>()
                .As<IHubNotification<LandmarkRunStatusNotification>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<HubNotification<InfoMessageNotificationHub, InfoMessageNotification>>()
                .As<IHubNotification<InfoMessageNotification>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<HubNotification<ScenarioNotificationHub, ScenarioNotificationModel>>()
                .As<IHubNotification<ScenarioNotificationModel>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ReportExportNotificationHub>()
                .AsSelf()
                .InstancePerLifetimeScope();

            // Register Environments Management for collectively managing environments
            _ = builder.Register(x =>
            {
                var mapper = x.Resolve<IMapper>();
                var autoBookSettings = x.Resolve<AutoBookSettings>();
                var environmentApi =
                    new AWSEnvironmentAPI(autoBookSettings.ProvisioningAPIURL, _autobookProviderApiAccessToken);

                return new AWSEnvironment(environmentApi, mapper);
            })
                .As<IEnvironment>();

            // Register Autobook Provider API
            _ = builder.Register(x =>
            {
                var autoBookSettings = x.Resolve<AutoBookSettings>();
                return new ProviderLogsAPI(autoBookSettings.ProvisioningAPIURL, _autobookProviderApiAccessToken);
            })
                .As<IProviderLogsAPI>();

            _ = builder.RegisterType<AutoBookCreateBackgroundJob>()
                .AsSelf()
                .As<IBackgroundJob>();

            _ = builder.RegisterType<AutoBookDeleteBackgroundJob>()
                .AsSelf()
                .As<IBackgroundJob>();

            _ = builder.RegisterType<RunCompletionBackgroundJob>()
                .AsSelf()
                .As<IBackgroundJob>();

            _ = builder.RegisterType<GenerateRecommendationsReportBackgroundJob>()
                .AsSelf()
                .As<IBackgroundJob>();

            _ = builder.RegisterType<LandmarkTriggerRunJob>()
                .AsSelf()
                .As<IBackgroundJob>();

            _ = builder.RegisterType<RunStatusChecker>().InstancePerDependency();

            if (_testEnvironment.HasOptions(TestEnvironmentOptions.LandmarkServicesStub))
            {
                _ = builder.RegisterType<LandmarkApiClientStub>()
                    .As<ILandmarkApiClient>()
                    .SingleInstance();

                _ = builder.RegisterType<LandmarkAutoBookPayloadProviderStub>()
                    .As<ILandmarkAutoBookPayloadProvider>()
                    .SingleInstance();
            }

            if (_testEnvironment.HasOptions(TestEnvironmentOptions.PerformanceLog))
            {
                _ = builder.Register(context =>
                        context.ResolveOptionalNamed<ILoggerFactory>(SqlServerAutofacModuleBase
                            .DbContextLoggerFactoryRegistrationName)?.CreateLogger<PerformanceLog>() ??
                        new NullLogger<PerformanceLog>())
                    .As<ILogger<PerformanceLog>>()
                    .InstancePerLifetimeScope();

                _ = builder.Register(context =>
                {
                    var tenantIdentifier = context.Resolve<TenantIdentifier>();
                    return new SerilogLoggerFactory(new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .Enrich.With<UtcTimeLogEventEnricher>()
                        .Enrich.With(new PerformanceLogCorrelationIdLogEventEnricher("perf-test-id"))
                        .WriteTo.Async(a =>
                            a.File(PerformanceLog.GetLogFileNameTemplate(tenantIdentifier),
                                outputTemplate: "{UtcTime}|[{Level:u3}]|{CorrelationId}|{Message:lj}{NewLine}{Exception}",
                                rollingInterval: RollingInterval.Day,
                                shared: true))
                        .CreateLogger(), true);
                })
                    .Named<ILoggerFactory>(SqlServerAutofacModuleBase.DbContextLoggerFactoryRegistrationName)
                    .SingleInstance();
            }
        }
    }
}
