using System;
using System.Globalization;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbSequence
{
    public class SqlServerIdentifierSequenceBase : ISqlServerIdentifierSequence
    {
        private readonly ISqlServerDbContext _dbContext;

        protected SqlServerIdentifierSequenceBase(ISqlServerDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public T GetNextValue<T>(string sequenceName) where T : IConvertible
        {
            var schema = _dbContext.Specific.Model.Relational()?.DefaultSchema;
            var connection = _dbContext.Specific.Database.GetDbConnection();
            _dbContext.Specific.Database.OpenConnection();
            using (var command = connection.CreateCommand())
            {
                var sequenceNameWithSchema =
                    (string.IsNullOrEmpty(schema) ? string.Empty : $"[{schema}].") + $"[{sequenceName}]";
                command.CommandText = $"SELECT NEXT VALUE FOR {sequenceNameWithSchema}";
                var identifier = command.ExecuteScalar();
                return (T) Convert.ChangeType(identifier, typeof(T), CultureInfo.InvariantCulture);
            }
        }
    }
}
