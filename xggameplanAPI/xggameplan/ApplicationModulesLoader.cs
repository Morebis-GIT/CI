using System;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Autofac.Multitenant;
using ImagineCommunications.Gameplan.Integration.Data.Entities;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Mappings;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping;
using Microsoft.Extensions.Configuration;
using NodaTime;
using Quartz;
using Quartz.Impl;
using xggameplan.Areas.System.Auth.Models;
using xggameplan.common.Caching;
using xggameplan.core.DependencyInjection;
using xggameplan.core.Extensions;
using xggameplan.core.FeatureManagement;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.Modules;
using xggameplan.SchedulerTasks;
using xggameplan.SchedulerTasks.Jobs;
using xggameplan.Services;
using xggameplan.TestEnv;
using xggameplan.Validations.Runs;
using xggameplan.Validations.Runs.Interfaces;

namespace xggameplan
{
    public static class ApplicationModulesLoader
    {
        private const string RUN_STATUS_CHECKER_GROUP = "R1";
        private static MultitenantContainer AppContainer { get; set; }

        public static void Configure(HttpConfiguration httpConfiguration, IConfiguration applicationConfiguration)
        {
            var builder = new ContainerBuilder();
            var baseContainer = CreateBaseContainer(builder, applicationConfiguration, httpConfiguration);
            var multitenantContainer = CreateMultitenantContainer(baseContainer, applicationConfiguration);
            httpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(multitenantContainer);

            baseContainer.ConfigureJobs(multitenantContainer);
        }

        public static void ConfigureJobs(this IContainer baseContainer, MultitenantContainer multitenantContainer)
        {
            var tenantsRepository = baseContainer.Resolve<ITenantsRepository>();
            var tenants = tenantsRepository.GetAll();

            var scheduler = baseContainer.Resolve<IScheduler>();
            scheduler.JobFactory = new MvcJobFactory(baseContainer, multitenantContainer, tenants);

            IJobDetail runner = JobBuilder
                .Create<RunStatusChecker>()
                .WithIdentity("run-status-checker", RUN_STATUS_CHECKER_GROUP)
                .Build();

            ITrigger runnerTrigger = TriggerBuilder.Create()
                .WithIdentity("run-status-checker-t", RUN_STATUS_CHECKER_GROUP)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(60)
                    .RepeatForever())
                .StartNow()
                .Build();

            _ = scheduler.ScheduleJob(runner, runnerTrigger);
            _ = scheduler.StartDelayed(TimeSpan.FromSeconds(20));
        }

        public static void SetupTenant(
            Tenant tenant,
            IConfiguration configuration,
            ITestEnvironment testEnvironment,
            IFeatureManager featureManager)
        {
            var rootFolder = System.Web.Hosting.HostingEnvironment.MapPath("/");
            AppContainer.ConfigureTenant(tenant.Id, builder =>
            {
                _ = builder.RegisterModule(new CoreAutofacModule(rootFolder));
                _ = builder.RegisterModule(new LandmarkRunServicesModule(configuration, featureManager));
                _ = builder.RegisterModule(
                    new DefaultAuditEventModule(System.Web.Hosting.HostingEnvironment.MapPath("/Logs")));
                _ = builder.RegisterModule(new TenantAutofacModule(tenant, configuration, testEnvironment,
                    featureManager));
            });
        }

        public static IDbContext GetTenantDbContext(int tenantId) =>
            AppContainer.GetTenantScope(tenantId).Resolve<IDbContext>();

        private static IContainer CreateBaseContainer(ContainerBuilder builder, IConfiguration configuration, HttpConfiguration httpConfiguration)
        {
            _ = builder.AddAutoMapper(
                typeof(AccessTokenModelProfile).Assembly,
                typeof(AutofacBuilderExtensions).Assembly,
                typeof(AccessTokenProfile).Assembly,
                typeof(SynchronizationProfile).Assembly,
                typeof(GroupTransactionInfoProfile).Assembly);

            _ = builder.RegisterInstance(SystemClock.Instance).As<IClock>();
            _ = builder.RegisterInstance(configuration).As<IConfiguration>().ExternallyOwned();
            _ = builder.RegisterType<InMemoryCache>().As<ICache>().InstancePerLifetimeScope();

            _ = builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(httpConfiguration);
            builder.RegisterHttpRequestMessage(httpConfiguration);

            _ = builder.RegisterModule(new TestEnvAutofacModule(configuration));
            _ = builder.RegisterModule<WebAutofacModule>();
            _ = builder.RegisterModule(new CloudModule(configuration));
            _ = builder.RegisterModule(new PersistenceAutofacModule(configuration));

            var scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
            _ = builder.RegisterInstance(scheduler);

            _ = builder.RegisterType<RunsValidator>().As<IRunsValidator>().InstancePerLifetimeScope();

            var container = builder.Build();

            return container;
        }

        private static MultitenantContainer CreateMultitenantContainer(IContainer baseContainer, IConfiguration applicationConfiguration)
        {
            using var scope = baseContainer.BeginLifetimeScope();
            var tenants = scope.Resolve<ITenantsRepository>().GetAll();
            var testEnvironment = scope.Resolve<ITestEnvironment>();
            var featureSettingsProvider = scope.Resolve<IFeatureSettingsProvider>();

            AppContainer = new MultitenantContainer(new TenantIdentificationStrategy(baseContainer), baseContainer);
            tenants.ForEach(tenant =>
            {
                var featureManager = new FeatureManager(featureSettingsProvider.GetForTenant(tenant.Id));
                SetupTenant(tenant, applicationConfiguration, testEnvironment, featureManager);
            });
            return AppContainer;
        }
    }
}
