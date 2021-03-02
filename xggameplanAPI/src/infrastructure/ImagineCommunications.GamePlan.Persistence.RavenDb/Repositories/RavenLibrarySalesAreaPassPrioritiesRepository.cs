using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenLibrarySalesAreaPassPrioritiesRepository : RavenRepositoryBase<LibrarySalesAreaPassPriority>,
        ILibrarySalesAreaPassPrioritiesRepository
    {
        public RavenLibrarySalesAreaPassPrioritiesRepository(IDocumentSession session) : base(session)
        {

        }

        /// <summary>
        /// Creates a Library Sales Area Pas Priority item based on the supplied entity
        /// </summary>
        /// <param name="entity">the entity of type <see cref="LibrarySalesAreaPassPriority"/></param>
        /// <returns></returns>
        public async Task AddAsync(LibrarySalesAreaPassPriority entity)
        {
            lock (_session)
            {
                entity.Uid = Guid.NewGuid();
                AddAuditForCreated(entity);
                AddAuditForModified(entity);
                _session.Store(entity);
            }
        }

        public async Task Delete(Guid id)
        {
            LibrarySalesAreaPassPriority item = await GetAsync(id).ConfigureAwait(false);
            if (item != null)
            {
                lock (_session)
                {
                    _session.Delete(item);
                }
            }
        }

        /// <summary>
        /// Upates a Library Sales Area Pas Priority item based on the supplied entity
        /// </summary>
        /// <param name="entity">the entity of type <see cref="LibrarySalesAreaPassPriority"/></param>
        /// <returns></returns>
        public async Task UpdateAsync(LibrarySalesAreaPassPriority entity)
        {
            lock (_session)
            {
                AddAuditForModified(entity);
                _session.Store(entity);
            }
        }

        /// <summary>
        /// To identify whether the supplied name Is Unique For Creating a new Library Sales Area Pas Priority item
        /// </summary>
        /// <param name="name">the name of the Library Sales Area Pas Priority item</param>
        /// <returns>True if the supplied name is unique or False otherwise</returns>
        public async Task<bool> IsNameUniqueForCreateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            name = name.ReduceExcessSpace();
            var exist = await ExistsAsync(a => a.Name == name.Trim());
            return !exist;
        }

        /// <summary>
        /// Identifies whether the supplied name Is Unique For Updating a Library Sales Area Pas Priority item
        /// </summary>
        /// <param name="name">the name of the Library Sales Area Pas Priority item</param>
        /// <param name="uId">The Uid of a Library Sales Area Pas Priority item </param>
        /// <returns>True if the supplied name is unique or False otherwise</returns>
        public async Task<bool> IsNameUniqueForUpdateAsync(string name, Guid uId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            name = name.ReduceExcessSpace();
            var exist = await ExistsAsync(a => a.Uid != uId && a.Name == name.Trim());
            return !exist;
        }

        /// <summary>
        /// Get a Library Sales Area Pas Priority item for the supplied uId
        /// </summary>
        /// <param name="uId">The Uid of the Library Sales Area Pas Priority item</param>
        /// <returns>A Library Sales Area Pas Priority item
        /// <see cref="LibrarySalesAreaPassPriority"/> if a matching record is found for the supplied uId
        /// </returns>
        /// <returns>Null if no matching record is found for the supplied uId </returns>
        public async Task<LibrarySalesAreaPassPriority> GetAsync(Guid uId)
        {
            LibrarySalesAreaPassPriority result;
            lock (_session)
            {
                result = _session.Query<LibrarySalesAreaPassPriority>().FirstOrDefault(a => a.Uid == uId);
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Get all Library Sales Area Pas Priority items
        /// </summary>
        /// <returns>All Library Sales Area Pas Priority items <see cref="IEnumerable<LibrarySalesAreaPassPriority>"/></returns>
        public async Task<IEnumerable<LibrarySalesAreaPassPriority>> GetAllAsync()
        {
            IEnumerable<LibrarySalesAreaPassPriority> result;
            lock (_session)
            {
                result = _session.GetAll<LibrarySalesAreaPassPriority>();
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Save all data changes
        /// </summary>
        /// <returns></returns>
        public async Task SaveChanges()
        {
            _session.SaveChanges();
        }
    }
}
