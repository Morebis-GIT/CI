using System.Reflection;
using Autofac;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Maintenance.DatabaseDetail;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Domain.Shared.System.Products;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.RavenDb.DbContext;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Services;
using Raven.Client;
using Raven.Client.FileSystem;
using xggameplan.common.Caching;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Repository;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.DependencyInjection
{
    public class RavenDbMasterModule : Autofac.Module
    {
        private readonly string _connectionString;

        public RavenDbMasterModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            IDocumentStore masterDocumentStore = DocumentStoreFactory.CreateStore(_connectionString, Assembly.GetExecutingAssembly());

            builder.RegisterInstance(masterDocumentStore)
                .Keyed<IDocumentStore>(DatabaseType.Master)
                .SingleInstance();

            builder.Register(context => context.ResolveKeyed<IDocumentStore>(DatabaseType.Master).OpenAsyncSession())
               .Keyed<IAsyncDocumentSession>(DatabaseType.Master)
               .InstancePerLifetimeScope()
               .OnRelease(instance => instance.Dispose());

            builder.Register(context => FileStoreFactory.CreateStore(_connectionString))
                .Keyed<IFilesStore>(DatabaseType.Master)
                .SingleInstance();

            builder.Register(x => x.ResolveKeyed<IFilesStore>(DatabaseType.Master).OpenAsyncSession())
                .As<IAsyncFilesSession>()
                .InstancePerLifetimeScope();

            builder.Register(x => x.Resolve<IAsyncFilesSession>())
                .Keyed<IAsyncFilesSession>(DatabaseType.Master)
                .InstancePerLifetimeScope();

            builder.Register(context =>
            {
                return context.ResolveKeyed<IDocumentStore>(DatabaseType.Master).OpenSession();
            })
                .Keyed<IDocumentSession>(DatabaseType.Master)
                .InstancePerLifetimeScope()
                .OnRelease(instance => instance.Dispose());

            builder.Register(context => new MasterRavenDbContext(
                    context.ResolveKeyed<IDocumentSession>(DatabaseType.Master),
                    context.ResolveKeyed<IAsyncDocumentSession>(DatabaseType.Master)))
                .As<IRavenMasterDbContext>()
                .As<IRavenDbContext>()
                .As<IMasterDbContext>()
                .As<IDbContext>()
                .InstancePerLifetimeScope();

            builder.Register(context =>
                new RavenPreviewFileStorage(context.ResolveKeyed<IFilesStore>(DatabaseType.Master)));

            builder.Register(context =>
                    new RavenUsersRepository(context.Resolve<IRavenMasterDbContext>(),
                                             context.Resolve<RavenPreviewFileStorage>()))
                .As<IUsersRepository>()
                .InstancePerLifetimeScope();

            builder.Register(context =>
                    new RavenUserSettingsRepository(context.Resolve<IUsersRepository>()))
                .As<IUserSettingsService>()
                .InstancePerLifetimeScope();

            builder.Register(context =>
                    new RavenTenantsRepository(context.Resolve<IRavenMasterDbContext>(),
                                              context.Resolve<RavenPreviewFileStorage>()))
                .As<ITenantsRepository>()
                .InstancePerLifetimeScope();

            builder.Register(context =>
                    new RavenAccessTokensRepository(context.Resolve<IRavenMasterDbContext>(), context.Resolve<ICache>()))
                .As<IAccessTokensRepository>()
                .InstancePerLifetimeScope();

            // Define Product Settings repository. Bit of a hack with the way
            // that it works at the moment
            builder.Register(context =>
                    new RavenProductSettingsRepository(context.Resolve<IRavenMasterDbContext>()))
                .As<IProductSettingsRepository>()
                .InstancePerLifetimeScope();

            // Define Task Instance repository. Bit of a hack with the way that
            // it works at the moment
            builder.Register(context =>
                    new RavenTaskInstanceRepository(context.Resolve<IRavenMasterDbContext>()))
                .As<ITaskInstanceRepository>()
                .InstancePerLifetimeScope();

            // Define Update Details repository
            builder.Register(context =>
                    new RavenUpdateDetailsRepository(context.Resolve<IRavenMasterDbContext>()))
                .As<IUpdateDetailsRepository>()
                .InstancePerLifetimeScope();

            // Define Database Details repository
            builder.Register(context =>
                    new RavenDatabaseDetailsRepository(context.Resolve<IRavenMasterDbContext>()))
                .As<IDatabaseDetailsRepository>()
                .InstancePerLifetimeScope();

            builder.Register(context =>
                    new RavenMasterIdentifierSequence(context.ResolveKeyed<IDocumentSession>(DatabaseType.Master)))
                .As<IRavenMasterIdentifierSequence>()
                .As<IRavenIdentifierSequence>()
                .As<IMasterIdentifierSequence>()
                .As<IIdentifierSequence>()
                .InstancePerLifetimeScope();

            builder.Register(context => new RavenDatabaseIndexAwaiter(context.Resolve<IRavenMasterDbContext>()))
                .As<IDatabaseIndexAwaiter>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RavenFeatureSettingsProvider>().As<IFeatureSettingsProvider>()
                .InstancePerLifetimeScope();
        }
    }
}
