using Autofac;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;

namespace xggameplan.core.Services
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly ILifetimeScope _lifetimeScope;

        public RepositoryFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public IRepositoryScope BeginRepositoryScope()
        {
            return new RepositoryScope(_lifetimeScope.BeginLifetimeScope());
        }
    }
}
