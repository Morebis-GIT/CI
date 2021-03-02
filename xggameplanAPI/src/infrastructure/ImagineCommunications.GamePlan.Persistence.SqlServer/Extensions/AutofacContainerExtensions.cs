using Autofac;
using Autofac.Builder;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions
{
    internal static class AutofacContainerExtensions
    {
        internal static IRegistrationBuilder<TImplementer, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterRepository<TImplementer, TService>(this ContainerBuilder builder)
        {
            return builder?.RegisterType<TImplementer>()
                .As<TService>()
                .WithSqlServerMetadata()
                .InstancePerLifetimeScope();
        }

        internal static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> WithSqlServerMetadata<TLimit,
            TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder)
        {
            return builder?.WithMetadata(DbMetadata.DbProviderKey, SqlServerMetadata.DbProviderName);
        }
    }
}
