using System;
using System.IO;
using System.Reflection;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.utils.seeddata.SqlServer.Migration.Entities;
using xggameplan.utils.seeddata.SqlServer.Migration.Interfaces;

namespace xggameplan.utils.seeddata.SqlServer.Migration
{
    public class MigrationHistoryInitializer : IMigrationPrepareAction
    {
        private const string TableNamePlaceholder = "{$TableName}";

        private readonly ISqlServerDbContext _dbContext;

        public MigrationHistoryInitializer(ISqlServerDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void Execute()
        {
            using (var streamReader = new StreamReader(Assembly.GetCallingAssembly()
                .GetManifestResourceStream($"{GetType().Namespace}.Scripts.MigrationHistoryTable.sql")))
            {
                var tableName = _dbContext.Specific.Model.GetEntityType<MigrationHistory>().Relational()?.TableName;
                var script = streamReader.ReadToEnd().Replace(TableNamePlaceholder, tableName);
                _ = _dbContext.Specific.Database.ExecuteSqlCommand(new RawSqlString(script));
            }
        }
    }
}
