using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Services
{
    public class SqlServerDatabaseIndexAwaiter : IDatabaseIndexAwaiter
    {
        private readonly ISqlServerDbContext _dbContext;

        protected static readonly Lazy<string> DbCommandText = new Lazy<string>(() =>
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"{typeof(SqlServerDatabaseIndexAwaiter).Namespace}.WaitForIndexes.sql")))
            {
                return reader.ReadToEnd();
            }
        }, LazyThreadSafetyMode.ExecutionAndPublication);

        protected async Task ExecuteDbCommandAsync(string indexName = null)
        {
            var p = new SqlParameter("@IndexName", SqlDbType.NVarChar)
            {
                Value = string.IsNullOrWhiteSpace(indexName) ? DBNull.Value : (object)indexName
            };

            await _dbContext.Specific.Database.ExecuteSqlCommandAsync(DbCommandText.Value, p).ConfigureAwait(false);
        }

        public SqlServerDatabaseIndexAwaiter(ISqlServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task WaitForIndexAsync(string indexName)
        {
            await ExecuteDbCommandAsync(indexName ?? throw new ArgumentNullException(nameof(indexName)))
                .ConfigureAwait(false);
        }

        public async Task WaitForIndexesAsync()
        {
            await ExecuteDbCommandAsync().ConfigureAwait(false);
        }
    }
}
