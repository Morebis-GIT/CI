using System.Collections.Generic;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemorySponsorshipRepository :
        MemoryRepositoryBase<Sponsorship>,
        ISponsorshipRepository

    {
        public MemorySponsorshipRepository()
        {
        }

        public Sponsorship Get(string externalReferenceId) => GetItemById(externalReferenceId);

        public IEnumerable<Sponsorship> GetAll() => GetAllItems();

        public void Add(Sponsorship item)
        {
            InsertOrReplaceItem(item, item.ExternalReferenceId);
        }

        public void Update(Sponsorship item)
        {
            InsertOrReplaceItem(item, item.ExternalReferenceId);
        }

        public void Delete(string externalReferenceId)
        {
            DeleteItem(externalReferenceId);
        }

        public void SaveChanges()
        {
        }

        public bool Exists(string externalReferenceId) => GetItemById(externalReferenceId) != null;

        public void Truncate()
        {
            DeleteAllItems();
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.CompletedTask;
        }
    }
}
