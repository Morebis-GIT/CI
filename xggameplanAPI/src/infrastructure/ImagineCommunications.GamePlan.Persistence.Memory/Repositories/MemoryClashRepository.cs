using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemoryClashRepository :
        MemoryRepositoryBase<Clash>,
        IClashRepository
    {
        public MemoryClashRepository()
        { }

        public void Dispose()
        { }

        public void Add(IEnumerable<Clash> items)
        {
            InsertItems(items.ToList(), items.Select(i => i.Uid.ToString()).ToList());
        }

        public void Add(Clash item)
        {
            var items = new List<Clash>() { item };
            InsertItems(items, items.Select(i => i.Uid.ToString()).ToList());
        }

        public Clash Get(Guid id)
        {
            return GetItemById(id.ToString());
        }

        [Obsolete("Use Get()")]
        public Clash Find(Guid id) => Get(id);

        public IEnumerable<Clash> FindByExternal(string externalref) =>
            GetAllItems(c => c.Externalref == externalref);

        public IEnumerable<Clash> FindByExternal(List<string> externalRefs) =>
            GetAllItems(c => externalRefs.Contains(c.Externalref));

        public IEnumerable<Clash> GetAll() => GetAllItems();

        public IEnumerable<ClashNameModel> GetDescriptionByExternalRefs(ICollection<string> externalRefs) => throw new NotImplementedException();

        public int Count() => GetCount();

        [Obsolete("Use Count()")]
        public int CountAll => Count();

        public void Delete(Guid uid)
        {
            DeleteItem(uid.ToString());
        }

        [Obsolete("Use Delete()")]
        public void Remove(Guid uid) => Delete(uid);

        public void Remove(Guid uid, out bool isDeleted)
        {
            isDeleted = false;
            var item = Find(uid);

            if (item is null)
            {
                return;
            }

            DeleteItem(uid.ToString());
            isDeleted = true;
        }

        public void Truncate()
        {
            DeleteAllItems();
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.FromResult(true);
        }

        public PagedQueryResult<ClashNameModel> Search(ClashSearchQueryModel queryModel) =>
            throw new NotImplementedException();

        public int Count(Expression<Func<Clash, bool>> query) => GetCount(query);

        public void SaveChanges() { }

        public bool Exists(Expression<Func<Clash, bool>> condition) => throw new NotImplementedException();

        public void DeleteRange(IEnumerable<Guid> ids) => throw new NotImplementedException();

        public void UpdateRange(IEnumerable<Clash> clashes) => throw new NotImplementedException();
    }
}
