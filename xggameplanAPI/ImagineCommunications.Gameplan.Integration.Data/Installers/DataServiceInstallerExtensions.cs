using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.Gameplan.Integration.Data.Context;
using ImagineCommunications.Gameplan.Integration.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ImagineCommunications.Gameplan.Integration.Data.Installers
{
    public static class DataServiceInstallerExtensions
    {
        public static IServiceCollection AddIntegrationPersistance(this IServiceCollection services, string connectionString)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<IntelligenceDbContext>()
                .UseMySql(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            services.AddSingleton(dbContextOptionsBuilder.Options);
            services.AddScoped<IntelligenceDbContext>();

            services.AddScoped<IGroupTransactionInfoRepository, GroupTransactionInfoRepository>();
            services.AddScoped<IMessageInfoRepository, MessageInfoRepository>();
            services.AddScoped<IMessagePayloadRepository, MessagePayloadRepository>();
            services.AddScoped<IMessagePriorityRepository, MessagePriorityRepository>();

            return services; 
        }
    }
}
