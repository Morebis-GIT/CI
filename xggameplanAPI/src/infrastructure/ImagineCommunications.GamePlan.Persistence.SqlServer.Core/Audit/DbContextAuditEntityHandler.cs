using System;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using NodaTime;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Audit
{
    public class DbContextAuditEntityHandler : IAuditEntityHandler
    {
        private readonly IClock _clock;

        public DbContextAuditEntityHandler(IClock clock)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        protected virtual AuditEntityState GetAuditEntityState<TEntity>(TEntity entity) 
            where TEntity : class, IAuditEntity
        {
            if (entity is null)
            {
                return AuditEntityState.None;
            }
            if (entity.DateCreated == DateTime.MinValue)
            {
                return AuditEntityState.Added;
            }

            return AuditEntityState.Modified;
        }

        public void AddAuditInfo<TEntity>(params TEntity[] entities) where TEntity : class, IAuditEntity
        {
            if (!(entities ?? throw new ArgumentNullException(nameof(entities))).Any())
            {
                return;
            }

            var timestamp = _clock.GetCurrentInstant().ToDateTimeUtc();
            foreach (var entity in entities)
            {
                AuditEntityHelper.SetAudit(entity, GetAuditEntityState(entity), timestamp);
            }
        }
    }
}
