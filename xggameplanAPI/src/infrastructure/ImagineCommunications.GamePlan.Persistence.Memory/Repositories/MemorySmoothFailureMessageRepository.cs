using System.Collections.Generic;
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

        public void Dispose()
        {
        }

        public IEnumerable<SmoothFailureMessage> GetAll() => GetAllItems();

        public void Add(IEnumerable<SmoothFailureMessage> items)
        {
            foreach (var item in items)
            {
                InsertOrReplaceItem(item, item.Id.ToString());
            }
        }

        public void Truncate()
        {
            DeleteAllItems();
        }

        public void SaveChanges()
        {
        }
    }
}
