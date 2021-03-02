using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public abstract class RavenRepositoryBase<T> where T: EntityBase
    {
        protected readonly IDocumentSession _session;

        public RavenRepositoryBase(IDocumentSession session)
        {
            _session = session;
        }

        /// <summary>
        ///  Identifies whether any record exists for the supplied condition
        /// </summary>
        /// <param name="condition">The condition of type <see cref="Expression<Func<T, bool>>"/></param>
        /// <returns>True if any record exists for the supplied condition or False otherwise</returns>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> condition)
        {
            bool exist;
            lock (_session)
            {
                exist = _session.Query<T>().Any(condition);
            }
            return await Task.FromResult(exist);
        }

        protected void AddAuditForCreated<TEntity>(TEntity entity) where TEntity : AuditBase
        {
            entity.DateCreated = DateTime.UtcNow;
        }

        protected void AddAuditForModified<TEntity>(TEntity entity) where TEntity : AuditBase
        {
            entity.DateModified = DateTime.UtcNow;
        }
    }
}
