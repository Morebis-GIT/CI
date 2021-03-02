using Autofac;
using ImagineCommunications.Gameplan.Synchronization.SqlServer;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Domain.Shared.System;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb.DependencyInjection;
using ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection;
using Microsoft.Extensions.Configuration;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.AutoBooks.AWS;
using xggameplan.common.Extensions;
using xggameplan.core.FeatureManagement.Interfaces;

namespace xggameplan.taskexecutor.DependencyInjection
{
    public class TenantModule : Module
    {
        private readonly IConfiguration _applicationConfiguration;
        private readonly IFeatureManager _featureManager;
        private readonly TaskInstance _taskInstance;
        private readonly Tenant _tenant;

        public TenantModule(
            IConfiguration applicationConfiguration,
            IFeatureManager featureManager,
            TaskInstance taskInstance,
            Tenant tenant)
        {
            _applicationConfiguration = applicationConfiguration;
            _featureManager = featureManager;
            _taskInstance = taskInstance;
            _tenant = tenant;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var autobookProviderApiAccessTokenParameter = "AutoBooks:AutobookProviderApiAccessToken";

            var tenantConnectionString = _applicationConfiguration.IsTempTenantDefined()
                ? _applicationConfiguration.GetTempTenantConnectionString()
                : null; //there is no possibility to determine tenant connection string without providing tenant info

            switch (_applicationConfiguration.GetDbProvider())
            {
                case DbProviderType.RavenDb:
                    _ = builder.RegisterModule(new RavenDbTenantModule(tenantConnectionString));
                    break;

                case DbProviderType.SqlServer:
                    _ = builder.RegisterModule(new SqlServerTenantModule(tenantConnectionString, _applicationConfiguration.GetValue<int>("SqlServerDb:LongRunningCommandsTimeout")));
                    _ = builder.RegisterModule(new SqlServerFeatureManagementModule(_featureManager));
                    _ = builder.RegisterModule(new SynchronizationServiceSqlServerServiceModule(tenantConnectionString));
                    break;
            }

            _ = builder.Register(x =>
            {
                var autoBookSettingsRepository = x.Resolve<IAutoBookSettingsRepository>();
                return autoBookSettingsRepository.Get();
            }).InstancePerLifetimeScope();

            _ = builder.Register<IAutoBooksAPI<AWSPAAutoBook, AWSPACreateAutoBook>>(x =>
              {
                  var autoBookSettings = x.Resolve<AutoBookSettings>();
                  var applicationConfiguration = x.Resolve<IConfiguration>();
                  var auditEventRepository = x.Resolve<IAuditEventRepository>();
                  return new AWSAutoBooksAPI(autoBookSettings.ProvisioningAPIURL,
                      applicationConfiguration[autobookProviderApiAccessTokenParameter], auditEventRepository);
              })
                .InstancePerLifetimeScope();

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
                    var applicationConfiguration = x.Resolve<IConfiguration>();

                    return new AWSAutoBooks(repositoryFactory, autoBookRepository, autoBookTypeRepository,
                        awsInstanceConfiguration,
                        auditEventRepository, autoBookSettings, autoBooksApi,
                        applicationConfiguration[autobookProviderApiAccessTokenParameter]);
                })
              .As<IAutoBooks>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterInstance(_taskInstance);
            _ = builder.RegisterInstance(new TenantIdentifier(_tenant.Id, _tenant.Name));
        }
    }
}
