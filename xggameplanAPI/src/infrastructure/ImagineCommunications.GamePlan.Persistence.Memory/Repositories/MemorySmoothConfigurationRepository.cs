using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemorySmoothConfigurationRepository :
        MemoryRepositoryBase<SmoothConfiguration>,
        ISmoothConfigurationRepository
    {
        public MemorySmoothConfigurationRepository()
        { }

        public void Dispose()
        { }

        public SmoothConfiguration GetById(int id) => GetItemById(id.ToString());

        public void Add(SmoothConfiguration item)
        {
            InsertOrReplaceItem(item, item.Id.ToString());
        }

        public void Update(SmoothConfiguration smoothConfiguration)
        {
            InsertOrReplaceItem(smoothConfiguration, smoothConfiguration.Id.ToString());
        }

        public void SaveChanges()
        {
        }

        public void Truncate()
        {
            DeleteAllItems();
        }
    }
}
