using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Truncate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer
{
    public class SqlServerTestTruncateHandler : TruncateHandler
    {
        private static readonly MethodInfo _deleteAsyncMethod = typeof(SqlServerTestTruncateHandler)
            .GetMethod(nameof(DeleteAsync), BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetGenericMethodDefinition();

        private readonly DbContext _dbContext;

        protected async Task DeleteAsync<TEntity>(CancellationToken cancellationToken)
            where TEntity : class
        {
            var entities = await _dbContext.Set<TEntity>().ToArrayAsync(cancellationToken).ConfigureAwait(false);
            _dbContext.RemoveRange(entities.Cast<object>());
        }

        protected override Task ExecuteDeleteAsync(IEntityType entityType, CancellationToken cancellationToken) =>
            (Task)_deleteAsyncMethod.MakeGenericMethod(entityType.ClrType)
                .Invoke(this, new object[] { cancellationToken });

        protected override Task ExecuteTruncateAsync(IEntityType entityType, CancellationToken cancellationToken) =>
            ExecuteDeleteAsync(entityType, cancellationToken);

        public SqlServerTestTruncateHandler(DbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
