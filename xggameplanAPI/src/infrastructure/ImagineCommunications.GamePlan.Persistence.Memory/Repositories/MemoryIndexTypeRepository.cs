using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.IndexTypes;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemoryIndexTypeRepository :
        MemoryRepositoryBase<IndexType>,
        IIndexTypeRepository
    {
        public MemoryIndexTypeRepository()
        {
        }

        public void Add(IEnumerable<IndexType> items)
        {
            InsertItems(items.ToList(), items.Select(i => i.Id.ToString()).ToList<string>());
        }

        public void Update(IndexType item)
        {
            UpdateOrInsertItem(item, item.Id.ToString());
        }

        public void Remove(int id)
        {
            DeleteItem(id.ToString());
        }

        public IndexType Find(int id) => GetItemById(id.ToString());

        public IEnumerable<IndexType> GetAll() => GetAllItems();

        public int CountAll => GetCount();

        public void Truncate()
        {
            DeleteAllItems();
        }

        public void SaveChanges()
        {
        }

        public void Dispose()
        {
        }
    }
}
