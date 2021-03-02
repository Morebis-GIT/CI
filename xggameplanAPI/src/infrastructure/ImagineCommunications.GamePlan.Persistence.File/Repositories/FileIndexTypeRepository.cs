using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileIndexTypeRepository : FileRepositoryBase, IIndexTypeRepository
    {
        public FileIndexTypeRepository(string folder) : base(folder, "index_type")
        {
        }

        public void Add(IEnumerable<IndexType> items)
        {
            InsertItems(_folder, _type, items.ToList(), items.Select(i => i.Id.ToString()).ToList());
        }

        public void Update(IndexType item)
        {
            UpdateOrInsertItem(_folder, _type, item, item.Id.ToString());
        }

        public void Remove(int id)
        {
            DeleteItem(_folder, _type, id.ToString());
        }

        public IndexType Find(int id)
        {
            return GetItemByID<IndexType>(_folder, _type, id.ToString());
        }

        public IEnumerable<IndexType> GetAll()
        {
            return GetAllByType<IndexType>(_folder, _type);
        }

        public int CountAll => CountAll(_folder, _type);

        public void Truncate()
        {
            DeleteAllItems(_folder, _type);
        }

        public void SaveChanges()
        {
        }

        public void Dispose()
        {
        }
    }
}
