using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using NodaTime;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Audit
{
    public class BulkInsertAuditPreProcessor : IBulkInsertEntityPreProcessor
    {
        private readonly IClock _clock;

        public BulkInsertOperation SupportedOperations => BulkInsertOperation.BulkInsert |
                                                          BulkInsertOperation.BulkInsertOrUpdate |
                                                          BulkInsertOperation.BulkUpdate;

        public BulkInsertAuditPreProcessor(IClock clock)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        protected virtual AuditEntityState GetAuditEntityState(IAuditEntity entity, BulkInsertOperation operation)
        {
            if (entity is null)
            {
                return AuditEntityState.None;
            }

            if ((operation & (BulkInsertOperation.BulkInsert | BulkInsertOperation.BulkInsertOrUpdate)) != 0)
            {
                if (entity.DateCreated == DateTime.MinValue)
                {
                    return AuditEntityState.Added;
                }

                return AuditEntityState.Modified;
            }

            if (operation == BulkInsertOperation.BulkUpdate)
            {
                return AuditEntityState.Modified;
            }

            return AuditEntityState.None;
        }

        public void Process<TEntity>(TEntity entity, BulkInsertOperation operation, BulkInsertOptions options)
            where TEntity : class
        {
            var timestamp = _clock.GetCurrentInstant().ToDateTimeUtc();
            if (entity is IAuditEntity auditEntity)
            {
                AuditEntityHelper.SetAudit(auditEntity, GetAuditEntityState(auditEntity, operation), timestamp);
            }
        }
    }
}
