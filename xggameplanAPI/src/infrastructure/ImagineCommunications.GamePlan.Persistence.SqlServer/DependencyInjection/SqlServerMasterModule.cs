using System;
using Autofac;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Domain.Shared.System.Products;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Services;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Repository;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection
{
    public class SqlServerMasterModule : SqlServerMasterModule<SqlServerMasterDbContext>
    {
        public SqlServerMasterModule(string connectionString) : base(connectionString)
        {
        }
    }

    public class SqlServerMasterModule<TDbContext> : SqlServerAutofacModuleBase
        where TDbContext : SqlServerMasterDbContext, ISqlServerMasterDbContext
    {
        private readonly string _connectionString;
        private readonly SqlServerDbContextRegistrationFeatures _features;

        public SqlServerMasterModule(string connectionString) :
            this(connectionString, new SqlServerDbContextRegistrationFeatures())
        {
        }

        public SqlServerMasterModule(string connectionString, SqlServerDbContextRegistrationFeatures features)
        {
            _connectionString = connectionString;
            _features = features ?? throw new ArgumentNullException(nameof(features));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(
                new SqlServerDbContextOptionsModule<TDbContext>(_connectionString, _features));

            builder.Register(context =>
                    new SqlServerDbContextFactory<ISqlServerMasterDbContext, TDbContext>(
                        context.Resolve<DbContextOptions<TDbContext>>()))
                .As<ISqlServerDbContextFactory<ISqlServerMasterDbContext>>()
                .InstancePerLifetimeScope();

            builder.Register(context => context.Resolve<ISqlServerDbContextFactory<ISqlServerMasterDbContext>>().Create())
                .As<ISqlServerMasterDbContext>()
                .As<ISqlServerDbContext>()
                .As<IMasterDbContext>()
                .As<IDbContext>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SqlServerMasterIdentifierSequence>()
                .As<ISqlServerMasterIdentifierSequence>()
                .As<ISqlServerIdentifierSequence>()
                .As<IMasterIdentifierSequence>()
                .As<IIdentifierSequence>()
                .InstancePerLifetimeScope();

            builder.Register(context => new SqlServerDatabaseIndexAwaiter(context.Resolve<ISqlServerMasterDbContext>()))
                .As<IDatabaseIndexAwaiter>()
                .InstancePerLifetimeScope();

            ////Add master database repositories registration below
            builder.RegisterType<AccessTokenRepository>().As<IAccessTokensRepository>();
            builder.RegisterType<UserRepository>().As<IUsersRepository>();
            builder.RegisterType<UserSettingsRepository>().As<IUserSettingsService>();
            builder.RegisterType<TenantsRepository>().As<ITenantsRepository>();
            builder.RegisterType<ProductSettingsRepository>().As<IProductSettingsRepository>();
            builder.RegisterType<TaskInstanceRepository>().As<ITaskInstanceRepository>();
            builder.RegisterType<UpdateDetailsRepository>().As<IUpdateDetailsRepository>();

            builder.RegisterType<SqlServerFeatureSettingsProvider>().As<IFeatureSettingsProvider>()
                .InstancePerLifetimeScope();
        }
    }
}
