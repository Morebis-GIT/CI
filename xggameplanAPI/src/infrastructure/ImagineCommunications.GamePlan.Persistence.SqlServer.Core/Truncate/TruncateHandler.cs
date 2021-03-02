using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Helpers;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Truncate
{
    public class TruncateHandler : ITruncateHandler
    {
        private readonly Microsoft.EntityFrameworkCore.DbContext _dbContext;

        protected virtual async Task ExecuteDeleteAsync(IEntityType entityType, CancellationToken cancellationToken)
        {
            _ = await _dbContext.Database.ExecuteSqlCommandAsync(
                new RawSqlString($"DELETE FROM {EntityTypeHelper.GetFullTableName(entityType)}"), cancellationToken)
                .ConfigureAwait(false);
        }

        protected virtual async Task ExecuteTruncateAsync(IEntityType entityType, CancellationToken cancellationToken)
        {
            _ = await _dbContext.Database.ExecuteSqlCommandAsync(
                    new RawSqlString($"TRUNCATE TABLE {EntityTypeHelper.GetFullTableName(entityType)}"), cancellationToken)
                .ConfigureAwait(false);
        }

        protected async Task InternalTruncateAsync(IEntityType entityType, DeleteFromOptions options, CancellationToken cancellationToken)
        {
            var fkeys = entityType.GetReferencingForeignKeys().ToList();
            if (fkeys.Any())
            {
                if (options.HasFlag(DeleteFromOptions.TruncateDependent))
                {
                    foreach (var fkey in fkeys.Where(x => x.DeleteBehavior == DeleteBehavior.Cascade))
                    {
                        var dependentEntityType = fkey.DeclaringEntityType;
                        await InternalTruncateAsync(dependentEntityType, options, cancellationToken).ConfigureAwait(false);
                    }
                }
                await ExecuteDeleteAsync(entityType, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await ExecuteTruncateAsync(entityType, cancellationToken).ConfigureAwait(false);
            }
        }

        public TruncateHandler(Microsoft.EntityFrameworkCore.DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task TruncateAsync(IEntityType entityType, DeleteFromOptions options = DeleteFromOptions.None,
            CancellationToken cancellationToken = default)
        {
            if (entityType is null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            var transaction = _dbContext.Database.CurrentTransaction == null &&
                              options.HasFlag(DeleteFromOptions.UseTransaction)
                ? await _dbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false)
                : null;
            try
            {
                await InternalTruncateAsync(entityType, options, cancellationToken).ConfigureAwait(false);
                transaction?.Commit();
            }
            finally
            {
                transaction?.Dispose();
            }
        }
    }
}
