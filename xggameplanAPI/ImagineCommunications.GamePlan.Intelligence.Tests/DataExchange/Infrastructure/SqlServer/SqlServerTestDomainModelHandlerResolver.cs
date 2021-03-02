using BoDi;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer
{
    public class SqlServerTestDomainModelHandlerResolver : IDomainModelHandlerResolver
    {
        private readonly IObjectContainer _objectContainer;

        public SqlServerTestDomainModelHandlerResolver(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        public IDomainModelHandler<TModel> Resolve<TModel>() where TModel : class
        {
            return _objectContainer.Resolve<IDomainModelHandler<TModel>>();
        }
    }
}
