using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContextOptions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder<TContext> UseAuditEntityHandler<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder, Func<TContext, IAuditEntityHandler> auditEventEntityHandlerFactory)
            where TContext : Microsoft.EntityFrameworkCore.DbContext, ISqlServerDbContext
        {
            var extension = optionsBuilder.Options.FindExtension<AuditEntityOptionsExtension>() ?? new AuditEntityOptionsExtension();
            extension.WithDbContextAuditEntityHandler(dbContext => auditEventEntityHandlerFactory((TContext)dbContext));
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            return optionsBuilder;
        }

        public static DbContextOptionsBuilder<TContext> UseAuditEntityHandler<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder, IAuditEntityHandler auditEventEntityHandler)
            where TContext : Microsoft.EntityFrameworkCore.DbContext, ISqlServerDbContext
        {
            return UseAuditEntityHandler(optionsBuilder, dbContext => auditEventEntityHandler);
        }

        public static DbContextOptionsBuilder<TContext> UseBulkInsertEngine<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder, Func<TContext, ISqlServerBulkInsertEngine> bulkInsertEngineFactory)
            where TContext : SqlServerDbContext
        {
            var extension = optionsBuilder.Options.FindExtension<BulkInsertEngineOptionsExtension>() ?? new BulkInsertEngineOptionsExtension();
            extension.WithBulkInsertEngine(dbContext => bulkInsertEngineFactory((TContext) dbContext));
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            return optionsBuilder;
        }

        public static DbContextOptionsBuilder<TContext> UseBulkInsertEngine<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder, ISqlServerBulkInsertEngine bulkInsertEngine)
            where TContext : SqlServerDbContext
        {
            return UseBulkInsertEngine(optionsBuilder, dbContext => bulkInsertEngine);
        }
    }
}
