using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemorySmoothFailureMessageRepository :
        MemoryRepositoryBase<SmoothFailureMessage>,
        ISmoothFailureMessageRepository
    {
        public MemorySmoothFailureMessageRepository()
        {
        }

        public void Dispose() { }

        public IEnumerable<SmoothFailureMessage> GetAll() => GetAllItems();

        public void Add(IEnumerable<SmoothFailureMessage> items)
        {
            InsertItems(items.ToList(), items.Select(i => i.Id.ToString()).ToList<string>());
        }

        public void Truncate()
        {
            DeleteAllItems();
        }

        public void SaveChanges() { }
    }
}
