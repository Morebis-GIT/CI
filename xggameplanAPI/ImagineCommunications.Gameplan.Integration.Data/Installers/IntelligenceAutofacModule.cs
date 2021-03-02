using Autofac;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.Gameplan.Integration.Data.Context;
using ImagineCommunications.Gameplan.Integration.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.Gameplan.Integration.Data.Installers
{
    public class IntelligenceAutofacModule : Module
    {
        private readonly string _connectionString;

        public IntelligenceAutofacModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<IntelligenceDbContext>()
                .UseMySql(_connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            _ = builder.RegisterInstance(dbContextOptionsBuilder.Options);
            _ = builder.RegisterType<IntelligenceDbContext>().InstancePerLifetimeScope();

            _ = builder.RegisterType<GroupTransactionInfoRepository>().As<IGroupTransactionInfoRepository>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<MessageInfoRepository>().As<IMessageInfoRepository>()
               .InstancePerLifetimeScope();

            _ = builder.RegisterType<MessagePayloadRepository>().As<IMessagePayloadRepository>()
               .InstancePerLifetimeScope();

            _ = builder.RegisterType<MessagePriorityRepository>().As<IMessagePriorityRepository>()
               .InstancePerLifetimeScope();
        }
    }
}
