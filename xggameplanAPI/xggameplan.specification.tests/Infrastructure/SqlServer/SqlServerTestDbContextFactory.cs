using BoDi;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.SqlServer
{
    public class SqlServerTestDbContextFactory<TDbContext, TDbContextImplementation> : SqlServerDbContextFactory<TDbContext, TDbContextImplementation>
        where TDbContext : ISqlServerDbContext
        where TDbContextImplementation : SqlServerTestDbContext, TDbContext
    {
        private readonly ObjectContainer _objectContainer;

        public SqlServerTestDbContextFactory(ObjectContainer objectContainer) : base(null)
        {
            _objectContainer = objectContainer;
        }

        protected override TDbContext CreateInternal()
        {
            var dbContext = _objectContainer.Resolve<TDbContext>();
            if (dbContext is TDbContextImplementation impl)
            {
                impl.CreatedByFactoryCounter++;
            }

            return dbContext;
        }
    }
}
