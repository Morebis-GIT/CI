using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemorySponsorshipRepository :
        MemoryRepositoryBase<Sponsorship>,
        ISponsorshipRepository

    {
        public MemorySponsorshipRepository() { }

        public Sponsorship Get(string externalReferenceId) => GetItemById(externalReferenceId);

        public IEnumerable<Sponsorship> GetAll() => GetAllItems();

        public void Add(Sponsorship item)
        {
            var items = new List<Sponsorship>() { item };
            InsertItems(items, items.Select(i => i.ExternalReferenceId).ToList());
        }

        public void Update(Sponsorship item)
        {
            UpdateOrInsertItem(item, item.ExternalReferenceId);
        }

        public void Delete(string externalReferenceId)
        {
            DeleteItem(externalReferenceId);
        }

        public void SaveChanges() { }

        public bool Exists(string externalReferenceId) => GetItemById(externalReferenceId) != null;

        private void Truncate()
        {
            DeleteAllItems();
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.FromResult(true);
        }
    }
}
