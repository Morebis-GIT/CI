using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContextOptions
{
    public class BulkInsertEngineOptionsExtension : IDbContextOptionsExtension
    {
        private Func<SqlServerDbContext, ISqlServerBulkInsertEngine> _bulkInsertEngineFactory;
        private long? _serviceProviderHashCode;

        public bool ApplyServices(IServiceCollection services)
        {
            return true;
        }

        public long GetServiceProviderHashCode()
        {
            if (!_serviceProviderHashCode.HasValue)
            {
                _serviceProviderHashCode = _bulkInsertEngineFactory?.GetHashCode() ?? 0L;
            }

            return _serviceProviderHashCode.Value;
        }

        public void Validate(IDbContextOptions options)
        {
        }

        public string LogFragment => string.Empty;

        public void WithBulkInsertEngine(Func<SqlServerDbContext, ISqlServerBulkInsertEngine> bulkInsertEngineFactory)
        {
            _bulkInsertEngineFactory = bulkInsertEngineFactory ??
                                       throw new ArgumentNullException(nameof(bulkInsertEngineFactory));
            _serviceProviderHashCode = null;
        }

        public ISqlServerBulkInsertEngine GetBulkInsertEngine(SqlServerDbContext dbContext)
        {
            return _bulkInsertEngineFactory != null
                ? _bulkInsertEngineFactory(dbContext ?? throw new ArgumentNullException(nameof(dbContext)))
                : null;
        }
    }
}
