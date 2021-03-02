using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Context;

namespace ImagineCommunications.Gameplan.Synchronization.SqlServer
{
    public static class SynchronizationServiceSqlServerServiceCollectionExtensions
    {
        public static IServiceCollection UseSqlServerForSynchronizationService(this IServiceCollection services, string connectionString)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<SynchronizationDbContext>()
                .UseSqlServer(connectionString);

            services.AddSingleton(dbContextOptionsBuilder.Options);
            services.AddScoped<SynchronizationDbContext>();

            services.AddScoped<ISynchronizationObjectRepository, SynchronizationObjectRepository>();

            return services;
        }
    }
}
