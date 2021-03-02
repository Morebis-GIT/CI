using System.Collections.Generic;
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
            foreach (var item in items)
            {
                InsertOrReplaceItem(item, item.Id.ToString());
            }
        }

        public void Update(IndexType item)
        {
            InsertOrReplaceItem(item, item.Id.ToString());
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
