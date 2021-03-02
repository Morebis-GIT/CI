using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert
{
    public class SqlServerBulkInsertEngineOptions
    {
        public ICollection<IBulkInsertEntityPreProcessor> PreProcessors { get; } =
            new List<IBulkInsertEntityPreProcessor>();

        public Func<BulkInsertOptions> BulkInsertDefaultOptionsFactory { get; set; } = () => new BulkInsertOptions();

        public ILoggerFactory LoggerFactory { get; set; }
    }
}
