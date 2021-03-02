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
                _ = builder.Register(context =>
                      {
                          var bulkInsertEngineOptionsBuilder =
                              new SqlServerBulkInsertEngineOptionsBuilder<SqlServerBulkInsertEngineOptions>();

                          var isLoggerFactoryResolved = ResolveDbContextLoggerFactory(context, out var loggerFactory);

                          if (_features.Logging && isLoggerFactoryResolved)
                          {
                              _ = bulkInsertEngineOptionsBuilder.UseLoggerFactory(loggerFactory);
                          }

                          if (_features.Audit)
                          {
                              var clock = ResolveClock(context);
                              _ = bulkInsertEngineOptionsBuilder
                                  .AddEntityPreProcessor(new BulkInsertAuditPreProcessor(clock));
                          }

                          return bulkInsertEngineOptionsBuilder.Options;
                      })
                    .InstancePerLifetimeScope();
            }

            _ = builder.Register(context =>
                  {
                      var dbContextOptionsBuilder = new DbContextOptionsBuilder<TDbContext>()
                          .UseSqlServer(_connectionString, x =>
                          {
                              if (_features.CommandTimeout != null)
                              {
                                  _ = x.CommandTimeout(_features.CommandTimeout);
                              }
                          })
                          .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                      var isLoggerFactoryResolved = ResolveDbContextLoggerFactory(context, out var loggerFactory);
                      if (_features.Logging && isLoggerFactoryResolved)
                      {
                          _ = dbContextOptionsBuilder.UseLoggerFactory(loggerFactory);
                      }

                      if (_features.Audit)
                      {
                          var clock = ResolveClock(context);
                          _ = dbContextOptionsBuilder.UseAuditEntityHandler(_ =>
                                new DbContextAuditEntityHandler(clock));
                      }

                      if (_features.BulkInsert)
                      {
                          var bulkInsertEngineOptions = context.Resolve<SqlServerBulkInsertEngineOptions>();
                          _ = dbContextOptionsBuilder.UseBulkInsertEngine(dbContext =>
                                new SqlServerBulkInsertEngine(dbContext, bulkInsertEngineOptions));
                      }

                      return dbContextOptionsBuilder.Options;
                  })
                .InstancePerLifetimeScope();
        }
    }
}
