using Autofac;
using Autofac.Builder;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Persistence.RavenDb.DependencyInjection;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions
{
    internal static class AutofacContainerExtensions
    {
        internal static IRegistrationBuilder<TImplementer, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterRepository<TImplementer, TService>(this ContainerBuilder builder)
        {
            return builder?.RegisterType<TImplementer>()
                .As<TService>()
                .WithRavenDbMetadata()
                .InstancePerLifetimeScope();
        }

        internal static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> WithRavenDbMetadata<TLimit,
            TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder)
        {
            return builder?.WithMetadata(DbMetadata.DbProviderKey, RavenDbMetadata.DbProviderName);
        }
    }
}
