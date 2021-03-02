using System;
using Autofac;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Audit;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection
{
    internal class SqlServerDbContextOptionsModule<TDbContext> : SqlServerAutofacModuleBase
        where TDbContext : SqlServerDbContext, ISqlServerDbContext
    {
        private readonly string _connectionString;
        private readonly SqlServerDbContextRegistrationFeatures _features;

        public SqlServerDbContextOptionsModule(string connectionString, SqlServerDbContextRegistrationFeatures features)
        {
            _connectionString = connectionString;
            _features = features ?? throw new ArgumentNullException(nameof(features));
        }

        protected override void Load(ContainerBuilder builder)
        {

            if (_features.BulkInsert)
            {
                builder.Register(context =>
                    {
                        var bulkInsertEngineOptionsBuilder =
                            new SqlServerBulkInsertEngineOptionsBuilder<SqlServerBulkInsertEngineOptions>();
                        var isLoggerFactoryResolved = ResolveDbContextLoggerFactory(context, out var loggerFactory);
                        if (_features.Logging && isLoggerFactoryResolved)
                        {
                            bulkInsertEngineOptionsBuilder.UseLoggerFactory(loggerFactory);
                        }

                        if (_features.Audit)
                        {
                            var clock = ResolveClock(context);
                            bulkInsertEngineOptionsBuilder
                                .AddEntityPreProcessor(new BulkInsertAuditPreProcessor(clock));
                        }

                        return bulkInsertEngineOptionsBuilder.Options;
                    })
                    .InstancePerLifetimeScope();
            }
            
            builder.Register(context =>
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<TDbContext>()
                        .UseMySql(_connectionString, x =>
                        {
                            if (_features.CommandTimeout != null)
                            {
                                x.CommandTimeout(_features.CommandTimeout);
                            }
                        })
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                    var isLoggerFactoryResolved = ResolveDbContextLoggerFactory(context, out var loggerFactory);
                    if (_features.Logging && isLoggerFactoryResolved)
                    {
                        dbContextOptionsBuilder.UseLoggerFactory(loggerFactory);
                    }

                    if (_features.Audit)
                    {
                        var clock = ResolveClock(context);
                        dbContextOptionsBuilder.UseAuditEntityHandler(dbContext =>
                            new DbContextAuditEntityHandler(clock));
                    }

                    if (_features.BulkInsert)
                    {
                        var bulkInsertEngineOptions = context.Resolve<SqlServerBulkInsertEngineOptions>();
                        dbContextOptionsBuilder.UseBulkInsertEngine(dbContext =>
                            new SqlServerBulkInsertEngine(dbContext, bulkInsertEngineOptions));
                    }

                    return dbContextOptionsBuilder.Options;
                })
                .InstancePerLifetimeScope();
        }
    }
}
