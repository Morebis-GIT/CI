using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContextOptions
{
    public class AuditEntityOptionsExtension : IDbContextOptionsExtension
    {
        private Func<ISqlServerDbContext, IAuditEntityHandler> _auditEntityHandlerFactory;
        private long? _serviceProviderHashCode;

        public bool ApplyServices(IServiceCollection services)
        {
            return true;
        }

        public long GetServiceProviderHashCode()
        {
            if (!_serviceProviderHashCode.HasValue)
            {
                _serviceProviderHashCode = _auditEntityHandlerFactory?.GetHashCode() ?? 0L;
            }

            return _serviceProviderHashCode.Value;
        }

        public void Validate(IDbContextOptions options)
        {
        }

        public string LogFragment => string.Empty;

        public void WithDbContextAuditEntityHandler(Func<ISqlServerDbContext, IAuditEntityHandler> auditEntityHandlerFactory)
        {
            _auditEntityHandlerFactory = auditEntityHandlerFactory ?? throw new ArgumentNullException(nameof(auditEntityHandlerFactory));
            _serviceProviderHashCode = null;
        }

        public IAuditEntityHandler GetAuditEntityHandler(ISqlServerDbContext dbContext)
        {
            return _auditEntityHandlerFactory != null
                ? _auditEntityHandlerFactory(dbContext ?? throw new ArgumentNullException(nameof(dbContext)))
                : null;
        }
    }
}
