using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Features.Metadata;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;

namespace xggameplan.core.Services
{
    public class IdentityGeneratorResolver : IIdentityGeneratorResolver
    {
        private readonly ILifetimeScope _lifetimeScope;

        public IdentityGeneratorResolver(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public IIdentityGenerator Resolve<T>() where T : class
        {
            var repositoryMeta = _lifetimeScope.ResolveOptional<Meta<Lazy<T>>>();
            if (repositoryMeta == null)
            {
                throw new InvalidOperationException($"There is no DI registration for '{typeof(T).Name}'.");
            }
            if (!repositoryMeta.Metadata.ContainsKey(DbMetadata.DbProviderKey))
            {
                throw new InvalidOperationException(
                    $"DI registration of '{typeof(T).Name}' does not contain 'dbProvider' metadata.");
            }

            var dbProvider = repositoryMeta.Metadata[DbMetadata.DbProviderKey];
            var identityGenerator = _lifetimeScope.Resolve<IEnumerable<Meta<Lazy<IIdentityGenerator>>>>()
                .Where(x => x.Metadata.ContainsKey(DbMetadata.DbProviderKey))
                .FirstOrDefault(x => x.Metadata[DbMetadata.DbProviderKey] == dbProvider);
            return identityGenerator?.Value.Value ??
                throw new InvalidOperationException($"Identity generator can not be resolved for '{typeof(T).Name}'.");
        }
    }
}
