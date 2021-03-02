using System.Collections.Generic;
using System.Linq;
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

        public void Add(SmoothConfiguration smoothConfiguration)
        {
            var items = new List<SmoothConfiguration>() {
                smoothConfiguration
            };

            InsertItems(
                items.ToList(),
                items.Select(i => i.Id.ToString()).ToList()
                );
        }

        public void Update(SmoothConfiguration smoothConfiguration)
        {
            UpdateOrInsertItem(smoothConfiguration, smoothConfiguration.Id.ToString());
        }

        public void SaveChanges() { }

        public void Truncate()
        {
            DeleteAllItems();
        }
    }
}
