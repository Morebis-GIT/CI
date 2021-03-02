using System;
using Autofac;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using xggameplan.utils.seeddata.SqlServer.DomainModelHandlers;

namespace xggameplan.utils.seeddata.SqlServer
{
    public class SeedDomainModelHandlerResolver : IDomainModelHandlerResolver
    {
        private readonly ILifetimeScope _lifetimeScope;

        public SeedDomainModelHandlerResolver(ILifetimeScope lifetimeScope) => _lifetimeScope =
            lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));

        public IDomainModelHandler<TModel> Resolve<TModel>() where TModel : class =>
            _lifetimeScope.ResolveOptional<IDomainModelHandler<TModel>>() ?? new FakeGenericDomainModelHandler<TModel>();
    }
}
