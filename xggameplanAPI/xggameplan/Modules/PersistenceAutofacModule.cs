using System;
using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using ImagineCommunications.GamePlan.Domain.Shared.System;
using ImagineCommunications.GamePlan.Persistence.RavenDb.DependencyInjection;
using ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Raven.Client;
using xggameplan.common.Extensions;
using xggameplan.PersistenceAttributes;
using xggameplan.Updates;

namespace xggameplan.Modules
{
    public class PersistenceAutofacModule : Module
    {
        private readonly IConfiguration _applicationConfiguration;

        public PersistenceAutofacModule(IConfiguration applicationConfiguration)
        {
            _applicationConfiguration = applicationConfiguration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Register master db
            var masterDbConnectionString = _applicationConfiguration.GetValue<string>("db:master:connectionString");
            var provider = _applicationConfiguration.GetDbProvider();
            switch (provider)
            {
                case DbProviderType.RavenDb:
                    builder.RegisterModule(new RavenDbMasterModule(masterDbConnectionString));
                    builder.Register(context =>
                         new RavenMasterSessionManagementAttribute(context.ResolveKeyed<IDocumentSession>(DatabaseType.Master)))
                     .AsWebApiActionFilterFor<ApiController>()
                     .InstancePerRequest();
                    break;

                case DbProviderType.SqlServer:
                    builder.RegisterModule<SqlServerInfrastructureModule>();
                    builder.RegisterModule(new SqlServerMasterModule(masterDbConnectionString));
                    builder.RegisterType<SqlServerDbContextManagementAttribute>()
                        .AsWebApiActionFilterFor<ApiController>()
                        .InstancePerRequest();
                    break;

                default:
                    throw new Exception("Unknown Master database provider.");
            }

            //it should be refactored within XGGT-9674
            var tenantDbConfig = _applicationConfiguration.GetSection("db:tempTenant");
            var tenantDbConnectionString = tenantDbConfig["connectionString"];

            builder.Register<IUpdateManager>(x =>
            {
                var updateDetailsRepository = x.Resolve<IUpdateDetailsRepository>();
                var mapper = x.Resolve<IMapper>();
                var tenantConnectionStrings = new List<string>()
                        {
                            tenantDbConnectionString
                        };

                string backupFolderRoot = HostingEnvironment.MapPath("/Updates");
                return new UpdateManager(updateDetailsRepository, masterDbConnectionString, tenantConnectionStrings, backupFolderRoot, mapper);
            });
        }
    }
}
