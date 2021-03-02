using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext
{
    public class SqlServerDbContextFactory<TDbContext, TDbContextImplementation> : ISqlServerDbContextFactory<TDbContext>
        where TDbContext : ISqlServerDbContext
        where TDbContextImplementation : SqlServerDbContext, TDbContext
    {
        private readonly Microsoft.EntityFrameworkCore.DbContextOptions _dbContextOptions;

        protected virtual TDbContext CreateInternal()
        {
            return (TDbContext) Activator.CreateInstance(typeof(TDbContextImplementation), _dbContextOptions);
        }

        public SqlServerDbContextFactory(Microsoft.EntityFrameworkCore.DbContextOptions dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public TDbContext Create()
        {
            return CreateInternal();
        }
    }
}
