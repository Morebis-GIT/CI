using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert
{
    public class EmptyGuidPkEntityPreProcessor : IBulkInsertEntityPreProcessor
    {
        public BulkInsertOperation SupportedOperations =>
            BulkInsertOperation.BulkInsert | BulkInsertOperation.BulkInsertOrUpdate;

        public void Process<TEntity>(TEntity entity, BulkInsertOperation operation, BulkInsertOptions options) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.GenerateValueForEmptyGuidPk && entity is IUniqueIdentifierPrimaryKey uiEntity)
            {
                if (uiEntity.Id == Guid.Empty)
                {
                    uiEntity.Id = Guid.NewGuid();
                }
            }
        }
    }
}
