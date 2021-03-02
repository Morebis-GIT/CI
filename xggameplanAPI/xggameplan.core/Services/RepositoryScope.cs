using System;
using System.Linq;
using Autofac;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;

namespace xggameplan.core.Services
{
    public class RepositoryScope : IRepositoryScope
    {
        private readonly ILifetimeScope _lifetimeScope;

        public RepositoryScope(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public IRepositoryScope BeginRepositoryScope()
        {
            return new RepositoryScope(_lifetimeScope.BeginLifetimeScope());
        }

        public RepositoryDictionary CreateRepositories(params Type[] repositoryTypes)
        {
            return new RepositoryDictionary(
                (repositoryTypes ?? Array.Empty<Type>()).ToDictionary(k => k, v => _lifetimeScope.Resolve(v)));
        }

        public TRepository CreateRepository<TRepository>() where TRepository : class
        {
            return _lifetimeScope.Resolve<TRepository>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _lifetimeScope.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
