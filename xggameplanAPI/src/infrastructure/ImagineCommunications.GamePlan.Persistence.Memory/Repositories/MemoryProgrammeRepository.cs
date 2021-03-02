using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Queries;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemoryProgrammeRepository :
        MemoryRepositoryBase<Programme>,
        IProgrammeRepository
    {
        public MemoryProgrammeRepository()
        {
        }

        public void Dispose()
        {
        }

        public void Add(IEnumerable<Programme> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Add(Programme item)
        {
            InsertOrReplaceItem(item, item.Id.ToString());
        }

        [Obsolete("Use Get()")]
        public Programme Find(Guid id) => Get(id);

        public void DeleteRange(IEnumerable<Guid> ids)
        {
            DeleteAllItems(b => ids.Contains(b.Id));
        }

        public Programme Get(Guid id)
        {
            return GetItemById(id.ToString());
        }

        public IEnumerable<Programme> Search(DateTime datefrom, DateTime dateto, string salesarea)
        {
            return GetAllItems(currentItem =>
                currentItem.SalesArea == salesarea &&
                currentItem.StartDateTime >= datefrom &&
                currentItem.StartDateTime <= dateto);
        }

        public PagedQueryResult<ProgrammeNameModel> Search(ProgrammeSearchQueryModel searchQuery)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Programme> GetAll()
        {
            return GetAllItems();
        }

        public int CountAll => GetCount();

        [Obsolete("Use Delete()")]
        public void Remove(Guid uid) => Delete(uid);

        public void Delete(Guid uid)
        {
            DeleteItem(uid.ToString());
        }

        public void Truncate()
        {
            DeleteAllItems();
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.CompletedTask;
        }

        public IEnumerable<Programme> FindByExternal(string externalRef)
        {
            return GetAllItems(p => p.ExternalReference == externalRef);
        }

        public IEnumerable<Programme> FindByExternal(List<string> externalRefs)
        {
            return GetAllItems(p => externalRefs.Contains(p.ExternalReference));
        }

        public int Count(Expression<Func<Programme, bool>> query) => GetCount(query);

        public void SaveChanges()
        {
        }

        public bool Exists(Expression<Func<Programme, bool>> condition) => throw new NotImplementedException();
    }
}
