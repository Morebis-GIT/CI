using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.IndexTypes
{
    public interface IIndexTypeRepository
    {
        void Add(IEnumerable<IndexType> items);

        IndexType Find(int id);

        IEnumerable<IndexType> GetAll();

        void Update(IndexType indexType);

        void Remove(int id);

        int CountAll { get; }

        void Truncate();

        void SaveChanges();
    }
}
