using Autofac;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;
using Microsoft.EntityFrameworkCore;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Context;

namespace ImagineCommunications.Gameplan.Synchronization.SqlServer
{
    public class SynchronizationServiceSqlServerServiceModule : Module
    {
        private readonly string _connectionString;

        public SynchronizationServiceSqlServerServiceModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<SynchronizationDbContext>()
                .UseMySql(_connectionString);

            builder.RegisterInstance(dbContextOptionsBuilder.Options).SingleInstance();
            builder.RegisterType<SynchronizationDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<SynchronizationObjectRepository>().As<ISynchronizationObjectRepository>().InstancePerLifetimeScope();
        }
    }
}
