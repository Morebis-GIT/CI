using System;
using System.Linq;
using System.Linq.Expressions;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Helpers
{
    public class SequenceRebuilder<TEntity, TSequence>
        where TEntity : class
        where TSequence : IIntIdentifier
    {
        public void Execute(ISqlServerDbContext dbContext, Expression<Func<TEntity, int>> entitySequencePropertyExpression)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (!dbContext.Specific.Database.IsSqlServer())
            {
                return;
            }

            var max = dbContext.Query<TEntity>()
                .Select(entitySequencePropertyExpression)
                .DefaultIfEmpty()
                .Max() + 1;

            var schema = dbContext.Specific.Model.Relational()?.DefaultSchema;
            var sequenceNameWithSchema = string.IsNullOrEmpty(schema)
                ? $"[{typeof(TSequence).Name}]"
                : $"[{schema}].[{typeof(TSequence).Name}]";
            var query = new RawSqlString($"ALTER SEQUENCE {sequenceNameWithSchema} RESTART WITH {max} NO CACHE");

            dbContext.Specific.Database.ExecuteSqlCommand(query);
        }
    }
}
