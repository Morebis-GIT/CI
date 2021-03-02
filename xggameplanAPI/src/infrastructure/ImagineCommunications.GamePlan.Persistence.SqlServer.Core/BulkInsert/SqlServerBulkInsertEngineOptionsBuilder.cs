using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert
{
    public class SqlServerBulkInsertEngineOptionsBuilder<TOptions>
        where TOptions : SqlServerBulkInsertEngineOptions, new()
    {
        public SqlServerBulkInsertEngineOptionsBuilder()
        {
            Options = new TOptions();
            Options.PreProcessors.Add(new EmptyGuidPkEntityPreProcessor());
        }

        public SqlServerBulkInsertEngineOptionsBuilder<TOptions> AddEntityPreProcessor(IBulkInsertEntityPreProcessor entityPreProcessor)
        {
            Options.PreProcessors.Add(entityPreProcessor ??
                                      throw new ArgumentNullException(nameof(entityPreProcessor)));
            return this;
        }

        public SqlServerBulkInsertEngineOptionsBuilder<TOptions> SetBulkInsertDefaultOptionsFactory(
            Func<BulkInsertOptions> factory)
        {
            Options.BulkInsertDefaultOptionsFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            return this;
        }

        public SqlServerBulkInsertEngineOptionsBuilder<TOptions> UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            Options.LoggerFactory = loggerFactory;
            return this;
        }

        public TOptions Options { get; }
    }
}
